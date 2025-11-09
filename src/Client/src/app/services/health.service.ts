import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class HealthService {
  apiStatus$ = new BehaviorSubject<boolean>(false);
  redisStatus$ = new BehaviorSubject<boolean>(false);
  driverServiceStatus$ = new BehaviorSubject<boolean>(false);
  driverDbStatus$ = new BehaviorSubject<boolean>(false);
  signalRStatus$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {}

  checkApi() {
    this.http.get(`${environment.shipmentServiceBaseUrl}/health`).subscribe({
      next: () => this.apiStatus$.next(true),
      error: () => this.apiStatus$.next(false),
    });
  }

  checkRedis() {
    this.http.get(`${environment.shipmentServiceBaseUrl}/health/redis`).subscribe({
      next: () => this.redisStatus$.next(true),
      error: () => this.redisStatus$.next(false),
    });
  }

  checkDriverService() {
    this.http.get(`${environment.driverServiceBaseUrl}/health`).subscribe({
      next: () => this.driverServiceStatus$.next(true),
      error: () => this.driverServiceStatus$.next(false),
    });
  }

  checkDriverDb() {
    this.http.get(`${environment.driverServiceBaseUrl}/health/db`).subscribe({
      next: () => this.driverDbStatus$.next(true),
      error: () => this.driverDbStatus$.next(false),
    });
  }

  setSignalRStatus(connected: boolean) {
    this.signalRStatus$.next(connected);
  }
}
