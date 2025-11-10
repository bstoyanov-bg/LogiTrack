import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Shipment, ShipmentService } from '../../../services/shipment.service';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { AssignDialogComponent } from '../assign-dialog/assign-dialog';

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
  shipments$!: Observable<Shipment[]>;

  constructor(private dialog: MatDialog, private shipmentService: ShipmentService) {}

  displayedColumns: string[] = ['id', 'trackingNumber', 'origin', 'destination', 'status', 'actions'];
  dataSource = [
    { id: 1, trackingNumber: 'TST001', origin: 'Sofia', destination: 'Varna', status: 'Pending' },
    { id: 2, trackingNumber: 'TST002', origin: 'Plovdiv', destination: 'Burgas', status: 'Delivered' }
  ];

  ngOnInit(): void {
    this.shipments$ = this.shipmentService.shipments$;
    this.shipmentService.loadShipments();
    this.shipmentService.initSignalR();
  }

  viewShipment(shipment: any) {
    console.log('View:', shipment);
  }

  deleteShipment(id: number) {
    console.log('Delete shipment with ID:', id);
  }

  assignDriver(shipment: any) {
  const dialogRef = this.dialog.open(AssignDialogComponent, {
    width: '400px',
    data: { shipmentId: shipment.id }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.shipmentService.assignDriver(result.shipmentId, result.driverId).subscribe(() => {
        console.log('Shipment assigned!');
      });
    }
  });
}
}
