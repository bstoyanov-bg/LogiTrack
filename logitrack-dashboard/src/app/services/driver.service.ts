import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface Driver {
  id: number;
  name: string;
  status: string;
  lastSeenUtc?: string;
}

@Injectable({ providedIn: 'root' })
export class DriverService {
  private apiUrl = `${environment.driverApiBaseUrl}/drivers`;

  constructor(private http: HttpClient) {}

  getDrivers(): Observable<Driver[]> {
    return this.http.get<Driver[]>(this.apiUrl);
  }

  getDriver(id: number): Observable<Driver> {
    return this.http.get<Driver>(`${this.apiUrl}/${id}`);
  }

  registerDriver(name: string): Observable<Driver> {
    return this.http.post<Driver>(`${this.apiUrl}/register`, { name });
  }

  updateStatus(id: number, status: string): Observable<Driver> {
    return this.http.put<Driver>(`${this.apiUrl}/${id}/status`, `"${status}"`, {
      headers: { 'Content-Type': 'application/json' },
    });
  }
}
