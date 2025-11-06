import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShipmentDashboard } from './shipment-dashboard';

describe('ShipmentDashboard', () => {
  let component: ShipmentDashboard;
  let fixture: ComponentFixture<ShipmentDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShipmentDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShipmentDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
