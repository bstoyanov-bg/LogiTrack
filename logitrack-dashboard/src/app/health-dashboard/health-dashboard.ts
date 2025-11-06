import { Component, OnInit } from '@angular/core';
import { HealthService } from '../services/health.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-health-dashboard',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './health-dashboard.html',
  styleUrls: ['./health-dashboard.scss']
})
export class HealthDashboardComponent implements OnInit {
  constructor(public health: HealthService) {}

  ngOnInit() {
    this.health.checkApi();
    this.health.checkRedis();
  }

  refresh() {
    this.health.checkApi();
    this.health.checkRedis();
  }
}
