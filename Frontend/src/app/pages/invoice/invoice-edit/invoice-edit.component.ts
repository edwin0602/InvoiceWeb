import { AuthService } from './../../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as saveAs from 'file-saver';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { Observable, of } from 'rxjs';
import { CustomerInvoiceService } from 'src/app/services/customerinvoice.service';
import { InvoicePaymentModalComponent } from '../invoice-payment-modal/invoice-payment-modal.component';

@Component({
  selector: 'app-invoice-edit',
  templateUrl: './invoice-edit.component.html',
  styleUrls: ['./invoice-edit.component.less'],
  standalone: false
})
export class InvoiceEditComponent implements OnInit {
  validateForm!: FormGroup;
  customersList: any[] = [];
  usersList: any[] = [];
  vatsList: any[] = [];
  invoiceTypeList: any[] = [];
  itemsList: any[] = [];
  invoiceFiles: any[] = [];
  invoiceItems: any[] = [];
  invoicePayments: any[] = [];
  invoiceNotes: any[] = [];
  subTotalAmount = 0;
  vatAmount = 0;
  totalAmount = 0;
  invoiceId: string = '';
  currentInvoice: any = null;
  currentUser: any;
  loading = true;
  isUploading = false;

  constructor(
    private fb: FormBuilder,
    private invoiceService: CustomerInvoiceService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService,
    private modal: NzModalService,
    private authservice: AuthService
  ) { }

  ngOnInit(): void {
    this.invoiceId =
      this.route.snapshot.paramMap.get('customerinvoiceId') || '';

    this.validateForm = this.fb.group({
      customerId: [null, [Validators.required]],
      userId: [null, [Validators.required]],
      invoiceDate: [null, [Validators.required]],
      vatId: [null],
      invoiceType: [null, [Validators.required]],
      status: ['Created', [Validators.required]],
      subTotal: [0],
      tax: [0],
      total: [0],
      paid: [0],
      lineItems: this.fb.array([]),
    });

    this.loadCustomers();
    this.loadUsers();
    this.loadVats();
    this.loadItems();
    this.loadInvoiceTypes();
    this.loadInvoiceData();
    this.currentUser = this.authservice.getUserInfo().username;
  }

  loadCustomers() {
    this.invoiceService.getCustomers().subscribe((data: any) => {
      this.customersList = data.items;
      this.loading = false;
    });
  }

  loadUsers() {
    this.invoiceService.getUsers().subscribe((data: any) => {
      this.usersList = data;
      this.loading = false;
    });
  }

  loadVats() {
    this.invoiceService.getVats().subscribe((data: any) => {
      this.vatsList = data;
      this.loading = false;
    });
  }

  loadInvoiceTypes() {
    this.invoiceService.getInvoiceTypes().subscribe((data: any) => {
      this.invoiceTypeList = data;
    });
  }

  loadItems() {
    this.invoiceService.getItems().subscribe((data: any) => {
      this.itemsList = data.items;
      this.loading = false;
    });
  }

  loadInvoiceData() {
    if (this.invoiceId) {
      this.invoiceService
        .getCustomerInvoiceById(this.invoiceId)
        .subscribe((invoice: any) => {
          this.validateForm.patchValue({
            customerId: invoice.customer_id,
            userId: this.authservice.getUserInfo().userId,
            invoiceDate: invoice.invoiceDate,
            vatId: invoice.vat_id,
            invoiceType: invoice.invoiceType,
            status: invoice.status,
            subTotal: invoice.subTotalAmount,
            tax: invoice.vatAmount,
            total: invoice.totalAmount,
            paid: invoice.paidAmount
          });

          invoice.customerInvoiceLines.forEach((line: any) => {
            this.addLineItemWithData(line);
          });

          this.invoiceItems = (invoice.customerInvoiceLines || []).map((line: any) => ({
            invoiceLineId: line.customerInvoiceFileId,
            name: line.itemName,
            description: line.itemDescription,
            quantity: line.quantity,
            price: line.price,
            total: line.quantity * line.price
          }));

          this.invoiceFiles = (invoice.customerInvoiceFiles || []).map((file: any) => ({
            uid: file.customerInvoiceFileId,
            name: file.fileName,
            status: 'done',
            url: file.filePath,
            customerInvoiceFileId: file.customerInvoiceFileId,
            fileType: file.fileType,
            creationDate: file.creationDate,
            updateDate: file.updateDate
          }));

          this.invoicePayments = (invoice.customerInvoicePayments || []).map((payment: any) => ({
            paymentId: payment.customerInvoicePayId,
            paymentDate: payment.paymentDate,
            status: payment.status,
            paymentMethod: payment.paymentMethod,
            description: payment.description,
            amount: payment.amount,
            userName: payment.userName
          }));

          this.invoiceNotes = (invoice.customerInvoiceNotes || []).map((note: any) => ({
            text: note.text,
            date: note.date,
            status: note.status
          }));

          this.currentInvoice = invoice;

          this.calculateTotals();
          this.loading = false;
        });

      this.loading = false;
    }
  }

  beforeUpload = (file: NzUploadFile): boolean => {
    this.isUploading = true;
    const fileObj = file.originFileObj || file;
    this.invoiceService.uploadInvoiceFile(this.invoiceId, fileObj).subscribe({
      next: () => {
        this.invoiceFiles = [...this.invoiceFiles, file];
        this.message.success('Soporte cargado con exito');
        this.isUploading = false;
      },
      error: (err) => {
        console.error('Error uploading file:', err);
        this.message.error('Error subiendo archivos');
        this.isUploading = false;
      }
    });

    return false;
  };

  onRemoveFile = (file: NzUploadFile): Observable<boolean> => {
    if (!file.uid) {
      this.message.error('No se encontró el identificador del archivo.');
      return of(false);
    }

    return new Observable<boolean>((observer) => {
      this.invoiceService.deleteInvoiceFile(this.invoiceId, file.uid).subscribe({
        next: () => {
          this.message.success('Archivo eliminado');
          observer.next(true);
          observer.complete();
        },
        error: (err) => {
          if (err.status === 404) {
            this.message.success('Archivo eliminado');
            observer.next(true);
          } else {
            this.message.error('No se pudo eliminar el archivo');
            observer.next(false);
          }
          observer.complete();
        }
      });
    });
  };

  onDownloadFile = (file: NzUploadFile) => {
    if (file.url) {
      console.log('Descargando archivo:', file);
      this.invoiceService.downloadInvoiceFile(this.invoiceId, file.uid).subscribe(res => {
        const blob = res.body!;

        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = file.name || 'archivo';
        a.click();

        URL.revokeObjectURL(a.href);
      });
    } else {
      this.message.info('El archivo aún no está disponible para descargar.');
    }
  };

  get lineItems(): FormArray {
    return this.validateForm.get('lineItems') as FormArray;
  }

  addLineItemWithData(lineItem: any) {
    const lineItemGroup = this.fb.group({
      item: [lineItem.item_id, Validators.required],
      description: [lineItem.itemDescription],
      quantity: [lineItem.quantity, Validators.required],
      price: [lineItem.price, Validators.required],
      total: [{ value: lineItem.quantity * lineItem.price, disabled: true }],
    });

    this.lineItems.push(lineItemGroup);
  }

  addLineItem() {
    const lineItemGroup = this.fb.group({
      item: [null, Validators.required],
      description: [''],
      quantity: [1, Validators.required],
      price: [0, Validators.required],
      total: [{ value: 0, disabled: true }],
    });

    this.lineItems.push(lineItemGroup);
  }

  removeLineItem(index: number) {
    this.lineItems.removeAt(index);
    this.calculateTotals();
  }

  calculateTotals() {
    this.subTotalAmount = this.lineItems.controls.reduce((sum, lineItem) => {
      const quantity = lineItem.get('quantity')?.value || 0;
      const price = lineItem.get('price')?.value || 0;
      const total = quantity * price;
      lineItem.get('total')?.setValue(total);
      return sum + total;
    }, 0);

    this.calculateVatAmount();
  }

  calculateVatAmount() {
    const vatId = this.validateForm.get('vatId')?.value;
    const vat = this.vatsList.find((v) => v.vatId === vatId);
    const percentage = vat ? vat.percentage : 0;
    this.vatAmount = this.subTotalAmount * (percentage / 100);
    this.totalAmount = this.subTotalAmount + this.vatAmount;

    this.validateForm.patchValue({
      tax: this.vatAmount.toFixed(2),
      total: this.totalAmount.toFixed(2),
      subTotal: this.subTotalAmount.toFixed(2),
    });
  }

  get canApplyPayments(): boolean {
    if (this.currentInvoice.invoiceType != "Final") return false;
    return (this.totalAmount - (this.validateForm.get('paidAmount')?.value || 0)) > 0;
  }

  onItemChange(index: number, itemId: string) {
    const selectedItem = this.itemsList.find((item) => item.itemId === itemId);
    if (selectedItem) {
      const lineItemGroup = this.lineItems.at(index);
      lineItemGroup.patchValue({
        description: selectedItem.description,
        price: selectedItem.salePrice,
      });
      this.calculateTotals();
    }
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      this.loading = true;

      const filesToUpload = this.invoiceFiles.filter(file => file.status !== 'done');
      if (filesToUpload.length > 0) {
        const uploadObservables = filesToUpload.map(file => {
          const fileObj = file.originFileObj || file;
          return this.invoiceService.uploadInvoiceFile(this.invoiceId, fileObj);
        });

        Promise.all(uploadObservables.map(obs => obs.toPromise()))
          .then(() => { this.updateCustomerInvoice(); })
          .catch(() => {
            this.loading = false;
            this.message.error('Error subiendo archivos');
          });
      } else {
        this.updateCustomerInvoice();
      }
    }
  }

  private updateCustomerInvoice(): void {
    const formValue = this.validateForm.value;
    const payload = {
      customer_id: formValue.customerId,
      user_id: formValue.userId,
      invoiceDate: formValue.invoiceDate,
      vat_id: formValue.vatId,
      invoiceType: formValue.invoiceType,
      status: formValue.status || 'Created',
      subTotalAmount: parseFloat(formValue.subTotal),
      vatAmount: parseFloat(formValue.tax),
      totalAmount: parseFloat(formValue.total),
      customerInvoiceLines: formValue.lineItems.map((item: any) => ({
        item_id: item.item,
        quantity: item.quantity,
        price: item.price,
      })),
    };

    this.invoiceService
      .updateCustomerInvoice(this.invoiceId, payload)
      .subscribe({
        next: () => {
          this.loading = false;
          this.message.success('Invoice updated successfully');
          this.router.navigate(['/invoices/all']);
        },
        error: (err) => {
          this.loading = false;
          this.message.error('Error updating invoice');
          console.error('Error updating customer:', err);
        }
      });
  }

  getUserName(userId: string): string {
    const user = this.usersList.find((u) => u.userId === userId);
    return user ? user.username : 'Unknown';
  }

  onBack(): void {
    this.router.navigate(['/invoices/all']);
  }

  openPaymentModal(): void {
    this.modal.create({
      nzTitle: 'Registrar nuevo pago',
      nzContent: InvoicePaymentModalComponent,
      nzData: {
        invoiceId: this.invoiceId,
        totalAmount: this.totalAmount,
        paidAmount: this.validateForm.get('paid')?.value || 0
      },
      nzFooter: null
    });
  }

  printInvoice(): void {
    this.invoiceService.makePdfInvoice(this.invoiceId).subscribe(
      (response: Blob) => {
        const blob = new Blob([response], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        const pdfWindow = window.open(url);
        if (pdfWindow) {
          pdfWindow.onload = () => {
            pdfWindow.focus();
            pdfWindow.print();
          };
        }
      },
      (error) => {
        this.message.error('Error generating the invoice PDF');
      }
    );
  }

  printInvoicePdf(): void {
    this.invoiceService.makePdfInvoice(this.invoiceId).subscribe(
      (response: Blob) => {
        saveAs(response, `Invoice_${this.invoiceId}.pdf`);
      },
      (error) => {
        this.message.error('Error downloading the PDF');
      }
    );
  }

  emailInvoicePdf(): void {
    this.invoiceService.emailInvoice(this.invoiceId).subscribe({
      next: (response) => {
        this.message.success(
          response.message || 'Invoice emailed successfully'
        );
      },
      error: (error) => {
        this.message.error(
          `Error sending invoice: ${error.error?.error || 'Unknown error'}`
        );
      },
    });
  }
}
