import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShipmentDashboardComponent } from './components/shipment-dashboard/shipment-dashboard';
import { ShipmentService } from './services/shipment.service';
import { HealthDashboardComponent } from './components/health-dashboard/health-dashboard';
import { DriversDashboardComponent } from './components/drivers-dashboard/drivers-dashboard';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, ShipmentDashboardComponent, HealthDashboardComponent, DriversDashboardComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent {
  title = 'LogiTrack Dashboard';

  constructor(private shipmentService: ShipmentService) {}

   ngOnInit() {
    this.shipmentService.loadShipments();  // Load initial data
    this.shipmentService.initSignalR();    // Connect to SignalR
  }
}
