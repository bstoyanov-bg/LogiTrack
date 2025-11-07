import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriversDashboard } from './drivers-dashboard';

describe('DriversDashboard', () => {
  let component: DriversDashboard;
  let fixture: ComponentFixture<DriversDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DriversDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DriversDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
