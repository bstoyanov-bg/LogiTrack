import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthService } from '../../services/health.service';

@Component({
  selector: 'app-health-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './health-dashboard.html',
  styleUrls: ['./health-dashboard.scss']
})
export class HealthDashboardComponent implements OnInit, OnDestroy {
  private intervalId: any;

  constructor(public health: HealthService) {}

  ngOnInit() {
    this.refresh();
    // Refresh on 10 seconds
    this.intervalId = setInterval(() => this.refresh(), 10000);
  }

  ngOnDestroy() {
    if (this.intervalId) clearInterval(this.intervalId);
  }

  refresh() {
    this.health.checkApi();
    this.health.checkRedis();
    this.health.checkDriverService();
    this.health.checkDriverDb();
  }
}
