import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { CustomerService } from 'src/app/services/customer.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DomSanitizer } from '@angular/platform-browser';
import { NzUploadFile, NzUploadXHRArgs } from 'ng-zorro-antd/upload';
import { Observable, of, Subscription } from 'rxjs';

@Component({
  selector: 'app-customer-edit',
  templateUrl: './customer-edit.component.html',
  styleUrls: ['./customer-edit.component.less'],
  standalone: false
})
export class CustomerEditComponent implements OnInit {
  editCustomerForm: FormGroup;
  customerId!: string;
  loading = true;
  statusesList: any[] = [];
  customerFiles: NzUploadFile[] = [];
  customerFileList: NzUploadFile[] = [];

  constructor(
    private fb: NonNullableFormBuilder,
    private customerService: CustomerService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService) {
    this.editCustomerForm = this.fb.group({
      customerId: new FormControl<string | null>({ value: null, disabled: true }),
      name: new FormControl<string | null>(null, [Validators.required]),
      address: new FormControl<string | null>(null),
      phoneNumber: new FormControl<string | null>(null),
      email: new FormControl<string | null>(null, [Validators.email]),
      status: new FormControl<string | null>(null),
      creationDate: new FormControl<string | null>({ value: null, disabled: true }),
      updateDate: new FormControl<string | null>({ value: null, disabled: true }),
    });
  }

  ngOnInit(): void {
    this.loadCustomerStatus();
    this.customerId = this.route.snapshot.paramMap.get('customerId')!;
    this.customerService.getCustomerById(this.customerId).subscribe((customer) => {
      this.customerFileList = (customer.customerFiles || []).map((file: any) => ({
        uid: file.id ? file.id.toString() : (file.uid ? file.uid.toString() : ''),
        name: file.name || 'archivo',
        status: 'done',
        url: file.url,
        id: file.id,
        type: file.type || '',
        size: file.size || 0
      }));

      this.editCustomerForm.patchValue({
        customerId: customer.customerId,
        name: customer.name,
        address: customer.address,
        phoneNumber: customer.phoneNumber,
        email: customer.email,
        status: customer.status,
        creationDate: customer.creationDate,
        updateDate: customer.updateDate,
      });
      this.loading = false;
    }, () => {
      this.message.error('Error loading customer details');
      this.loading = false;
    });
  }

  onBack(): void {
    this.router.navigate(['/customers/all']);
  }

  loadCustomerStatus() {
    this.customerService.getCustomerStatus().subscribe((data: any) => {
      this.statusesList = data;
    });
  }

  beforeUpload = (file: NzUploadFile & { rawFile?: File }): boolean => {
    this.customerFiles = this.customerFiles.concat(file);
    return false;
  };


  onRemoveFile = (file: NzUploadFile): Observable<boolean> => {
    if (!file.uid) {
      this.message.error('No se encontró el identificador del archivo.');
      return of(false);
    }

    return new Observable<boolean>((observer) => {
      this.customerService.deleteCustomerFile(this.customerId, file.uid).subscribe({
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

  submitForm(): void {
    if (this.editCustomerForm.valid) {
      this.loading = true;
      const filesToUpload = this.customerFiles;
      console.log('Files to upload:', this.customerFiles);

      this.customerFiles.forEach((file: any) => {
        this.customerService.uploadCustomerFile(this.customerId, file as File);
      });

      if (filesToUpload.length > 0) {
        const uploadObservables = filesToUpload.map(file => {
          return this.customerService.uploadCustomerFile(this.customerId, file.originFileObj as File);
        });

        Promise.all(uploadObservables.map(obs => obs.toPromise()))
          .then(() => {
            this.updateCustomer();
          })
          .catch(() => {
            this.loading = false;
            this.message.error('Error subiendo archivos');
          });
      } else {
        this.updateCustomer();
      }
    }
  }

  private updateCustomer(): void {
    const updatedCustomer = {
      customerId: this.customerId,
      ...this.editCustomerForm.value
    };

    this.customerService.updateCustomer(this.customerId, updatedCustomer).subscribe({
      next: () => {
        this.loading = false;
        this.message.success('Customer updated successfully');
        this.router.navigate(['/customers/all']);
      },
      error: (err) => {
        this.loading = false;
        this.message.error('Error updating customer');
        console.error('Error updating customer:', err);
      }
    });
  }

}
