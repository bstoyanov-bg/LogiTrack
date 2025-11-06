import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { HealthService } from './health.service';

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
  private signalRConnected = new BehaviorSubject<boolean>(false);
  signalRConnected$ = this.signalRConnected.asObservable();

  constructor(private http: HttpClient, private health: HealthService) {}

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
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(environment.signalRHubUrl)
    .build();

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

   // Start connection and track status
   this.hubConnection.start()
    .then(() => {
      console.log('✅ SignalR connected');
      this.health.setSignalRStatus(true); // 🔹 update dashboard
    })
    .catch(err => {
      console.error('❌ SignalR connection failed', err);
      this.health.setSignalRStatus(false);
     });
   }

  /** Stop connection (optional, e.g. when user logs out) */
  stopSignalR() {
    this.hubConnection?.stop();
  }
}
