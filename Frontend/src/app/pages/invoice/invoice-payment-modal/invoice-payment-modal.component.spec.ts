import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePaymentModalComponent } from './invoice-payment-modal.component';

describe('InvoicePaymentModalComponent', () => {
  let component: InvoicePaymentModalComponent;
  let fixture: ComponentFixture<InvoicePaymentModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoicePaymentModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoicePaymentModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
