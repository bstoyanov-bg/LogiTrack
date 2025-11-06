import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class HealthService {
  apiStatus$ = new BehaviorSubject<boolean>(false);
  redisStatus$ = new BehaviorSubject<boolean>(false);
  signalRStatus$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {}

  checkApi() {
    this.http.get(`${environment.apiBaseUrl}/health`).subscribe({
      next: () => this.apiStatus$.next(true),
      error: () => this.apiStatus$.next(false),
    });
  }

  checkRedis() {
    this.http.get(`${environment.apiBaseUrl}/health/redis`).subscribe({
      next: () => this.redisStatus$.next(true),
      error: () => this.redisStatus$.next(false),
    });
  }

  setSignalRStatus(connected: boolean) {
    this.signalRStatus$.next(connected);
  }
}
