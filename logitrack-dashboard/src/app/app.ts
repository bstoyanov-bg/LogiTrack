import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShipmentDashboardComponent } from './components/shipment-dashboard/shipment-dashboard';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, ShipmentDashboardComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent {
  title = 'LogiTrack Dashboard';
}
