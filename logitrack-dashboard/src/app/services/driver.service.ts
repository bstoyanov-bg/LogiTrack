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
  private api = environment.driverApiBaseUrl || environment.apiBaseUrl.replace('/api','') + '/api/drivers';
  constructor(private http: HttpClient) {}

  getDrivers(): Observable<Driver[]> {
    return this.http.get<Driver[]>(this.api);
  }

  getDriver(id: number): Observable<Driver> {
    return this.http.get<Driver>(`${this.api}/${id}`);
  }
}
