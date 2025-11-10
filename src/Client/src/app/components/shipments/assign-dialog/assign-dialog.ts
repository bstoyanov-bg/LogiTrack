import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Driver, DriverService } from '../../../services/driver.service';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';



@Component({
  selector: 'app-assign-dialog',
  standalone: true,
   imports: [
    CommonModule,
    MatSelectModule,
    MatDialogModule
  ],
  templateUrl: './assign-dialog.html',
  styleUrls: ['./assign-dialog.scss'],
})
export class AssignDialogComponent implements OnInit {
  drivers: Driver[] = [];
  selectedDriverId?: number;
  loading = true;

  constructor(
    private driverService: DriverService,
    private dialogRef: MatDialogRef<AssignDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { shipmentId: number }
  ) {}

  ngOnInit(): void {
    this.driverService.getDrivers().subscribe({
      next: (res) => {
        this.drivers = res;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  confirmAssignment(): void {
    if (!this.selectedDriverId) return;
    this.dialogRef.close({
      shipmentId: this.data.shipmentId,
      driverId: this.selectedDriverId,
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
