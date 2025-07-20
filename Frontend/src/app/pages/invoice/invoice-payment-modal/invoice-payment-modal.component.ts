import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { AuthService } from 'src/app/services/auth.service';
import { BankAccountService } from 'src/app/services/bank-account.service';
import { CustomerInvoiceService } from 'src/app/services/customerinvoice.service';

@Component({
  selector: 'app-invoice-payment-modal',
  templateUrl: './invoice-payment-modal.component.html',
  styleUrl: './invoice-payment-modal.component.less',
  standalone: false
})
export class InvoicePaymentModalComponent implements OnInit {
  invoiceId!: string;
  totalAmount!: number;
  paidAmount!: number;
  paymentMethods: any[] = [];
  paymentForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private modalRef: NzModalRef,
    private message: NzMessageService,
    private auth: AuthService,
    private accountService: BankAccountService,
    private paymentService: CustomerInvoiceService
  ) {
    const data = this.modalRef.getConfig().nzData;
    this.invoiceId = data.invoiceId;
    this.totalAmount = data.totalAmount;
    this.paidAmount = data.paidAmount;
  }

  ngOnInit(): void {
    this.paymentForm = this.fb.group({
      paymentDate: [new Date(), Validators.required],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      paymentMethod: [null, Validators.required],
      userId: [this.auth.getUserInfo()?.userId, Validators.required],
      description: ['']
    });

    this.getPaymentMethods();
  }

  setTotalAmount(): void {
    const maxAmount = this.totalAmount - this.paidAmount;
    this.paymentForm.get('amount')?.setValue(maxAmount);
  }

  getPaymentMethods(): void {
    const filters = [{ key: 'status', value: ['Active'] }];
    this.accountService.getAccounts(0, 100, null, null, filters).subscribe({
      next: (accounts) => {
        this.paymentMethods = accounts.map((account: any) => ({
          label: account.accountNumber + ' - ' + account.bankName,
          value: account.accountNumber + ' - ' + account.bankName
        }));
      },
      error: () => {
        this.message.error('Fail at load payment methods. Try again later.');
        this.modalRef.close(false);
      }
    });
  }

  submit(): void {
    if (this.paymentForm.valid) {
      const payload = {
        customerInvoiceId: this.invoiceId,
        ...this.paymentForm.value
      };
      this.paymentService.createPayment(payload).subscribe({
        next: () => {
          this.message.success('Payment applied successfully.');
          this.modalRef.close(false);
        },
        error: () => {
          this.message.error('Fail at apply payment. Try again later.');
          this.modalRef.close(false);
        }
      });
    }
  }
}
