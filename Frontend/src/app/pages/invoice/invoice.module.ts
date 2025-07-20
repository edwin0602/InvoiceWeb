import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InvoiceRoutingModule } from './invoice-routing.module';
import { InvoiceListComponent } from './invoice-list/invoice-list.component';
import { InvoiceAddComponent } from './invoice-add/invoice-add.component';
import { InvoiceEditComponent } from './invoice-edit/invoice-edit.component';
import { InvoicePaymentModalComponent } from './invoice-payment-modal/invoice-payment-modal.component';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { ReactiveFormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { InvoiceShowComponent } from './invoice-show/invoice-show.component';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzTabsModule, NzTabSetComponent } from "ng-zorro-antd/tabs";
import { SharedModule } from 'src/app/shared/shared.module';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTagModule } from 'ng-zorro-antd/tag';

@NgModule({
  declarations: [
    InvoiceListComponent,
    InvoiceAddComponent,
    InvoiceEditComponent,
    InvoiceShowComponent,
    InvoicePaymentModalComponent
  ],
  imports: [
    CommonModule,    
    ReactiveFormsModule,
    InvoiceRoutingModule,
    NzTableModule,
    NzIconModule,
    NzSpaceModule,
    NzDividerModule,
    NzPageHeaderModule,
    NzButtonModule,
    NzInputModule,
    NzFormModule,
    NzSelectModule,
    NzDatePickerModule,
    NzSpinModule,
    NzCardModule,
    NzModalModule,
    NzUploadModule,
    NzTagModule,
    NzTabsModule,
    NzTabSetComponent,
    SharedModule,
  ],
})
export class InvoiceModule { }
