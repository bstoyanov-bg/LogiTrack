import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-shipment-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './shipment-dashboard.html',
  styleUrl: './shipment-dashboard.scss'
})
export class ShipmentDashboardComponent implements OnInit {

  displayedColumns: string[] = ['id', 'trackingNumber', 'origin', 'destination', 'status', 'actions'];
  dataSource = [
    { id: 1, trackingNumber: 'TST001', origin: 'Sofia', destination: 'Varna', status: 'Pending' },
    { id: 2, trackingNumber: 'TST002', origin: 'Plovdiv', destination: 'Burgas', status: 'Delivered' }
  ];

  ngOnInit(): void {
    console.log('ShipmentDashboardComponent loaded');
  }

  viewShipment(shipment: any) {
    console.log('View:', shipment);
  }

  deleteShipment(id: number) {
    console.log('Del with ID:', id);
  }
}
