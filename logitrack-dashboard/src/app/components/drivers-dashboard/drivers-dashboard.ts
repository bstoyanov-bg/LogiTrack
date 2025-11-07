import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Driver, DriverService } from '../../services/driver.service';

@Component({
  selector: 'app-drivers-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatSelectModule,
    MatTableModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './drivers-dashboard.html',
  styleUrls: ['./drivers-dashboard.scss']
})
export class DriversDashboardComponent implements OnInit {
  drivers: Driver[] = [];
  loading = false;
  newDriverName = '';
  statuses = ['Available', 'Busy', 'Resting'];

  displayedColumns = ['id', 'name', 'status'];

  constructor(private driverService: DriverService) {}

  ngOnInit() {
    this.loadDrivers();
  }

  loadDrivers() {
    this.loading = true;
    this.driverService.getDrivers().subscribe({
      next: (data) => {
        this.drivers = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('❌ Failed to load drivers', err);
        this.loading = false;
      },
    });
  }

  registerDriver() {
    if (!this.newDriverName.trim()) return;
    this.driverService.registerDriver(this.newDriverName).subscribe({
      next: (driver) => {
        this.drivers = [...this.drivers, driver];
        this.newDriverName = '';
      },
      error: (err) => console.error('❌ Register failed', err),
    });
  }

  updateStatus(driver: Driver, newStatus: string) {
    this.driverService.updateStatus(driver.id, newStatus).subscribe({
      next: (updated) => {
        driver.status = updated.status;
      },
      error: (err) => console.error('❌ Status update failed', err),
    });
  }
}
