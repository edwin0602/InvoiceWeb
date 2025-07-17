import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReactiveFormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { AccountRoutingModule } from './account-routing.module';
import { AccountAddComponent } from './account-add/account-add.component';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { AccountListComponent } from './account-list/account-list.component';
import { NzOptionComponent, NzSelectComponent } from 'ng-zorro-antd/select';


@NgModule({
  declarations: [
    AccountAddComponent,
    AccountEditComponent,
    AccountListComponent
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    ReactiveFormsModule,
    NzTableModule,
    NzIconModule,
    NzSpaceModule,
    NzDividerModule,
    NzPageHeaderModule,
    NzButtonModule,
    NzInputModule,
    NzFormModule,
    NzNotificationModule,
    NzMessageModule,
    NzModalModule,
    NzOptionComponent,
    NzSelectComponent

  ]
})
export class AccountModule { }
