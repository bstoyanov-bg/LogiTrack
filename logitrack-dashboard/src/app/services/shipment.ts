import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Shipment {
  id: number;
  trackingNumber: string;
  origin: string;
  destination: string;
  status: number;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ShipmentService {
  private shipmentsSubject = new BehaviorSubject<Shipment[]>([]);
  shipments$ = this.shipmentsSubject.asObservable();
  private hubConnection?: HubConnection;

  constructor(private http: HttpClient) {}

  /** Load initial shipment list from backend API */
  loadShipments() {
    this.http.get<Shipment[]>(`${environment.apiBaseUrl}/Shipments`)
      .subscribe({
        next: (data) => this.shipmentsSubject.next(data),
        error: (err) => console.error('❌ Failed to load shipments', err)
      });
  }

  /** Initialize real-time SignalR connection */
  initSignalR() {
    if (this.hubConnection) return; // Prevent double connection

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl)
      .configureLogging(LogLevel.Information)
      .build();

    // Handle events from backend
    this.hubConnection.on('ShipmentCreated', (shipment: Shipment) => {
      const current = this.shipmentsSubject.value;
      this.shipmentsSubject.next([...current, shipment]);
    });

    this.hubConnection.on('ShipmentUpdated', (shipment: Shipment) => {
      const updated = this.shipmentsSubject.value.map(s => s.id === shipment.id ? shipment : s);
      this.shipmentsSubject.next(updated);
    });

    this.hubConnection.on('ShipmentDeleted', (id: number) => {
      const filtered = this.shipmentsSubject.value.filter(s => s.id !== id);
      this.shipmentsSubject.next(filtered);
    });

    this.hubConnection
      .start()
      .then(() => console.log('✅ SignalR connected to ShipmentHub'))
      .catch(err => console.error('❌ SignalR connection failed:', err));
  }

  /** Stop connection (optional, e.g. when user logs out) */
  stopSignalR() {
    this.hubConnection?.stop();
  }
}
