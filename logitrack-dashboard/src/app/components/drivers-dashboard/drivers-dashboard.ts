import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Driver, DriverService } from '../../services/driver.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-drivers-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatProgressSpinnerModule],
  templateUrl: './drivers-dashboard.html'
})
export class DriversDashboardComponent implements OnInit {
  drivers: Driver[] = [];
  loading = false;

  displayedColumns = ['id', 'name', 'status'];

  constructor(private driverService: DriverService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;
    this.driverService.getDrivers().subscribe({
      next: d => { this.drivers = d; this.loading = false; },
      error: e => { console.error(e); this.loading = false; }
    });
  }
}
