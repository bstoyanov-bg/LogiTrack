import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Driver, DriverService } from '../../services/driver.service';


@Component({
  selector: 'app-drivers-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './drivers-dashboard.html'
})
export class DriversDashboardComponent implements OnInit {
  drivers: Driver[] = [];
  loading = false;
  
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
