import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzTableQueryParams } from 'ng-zorro-antd/table';
import { BankAccountService } from 'src/app/services/bank-account.service';

@Component({
  selector: 'app-account-list',
  templateUrl: './account-list.component.html',
  styleUrls: ['./account-list.component.css'],
  standalone: false
})
export class AccountListComponent implements OnInit {
  total = 0;
  listOfAccounts: any[] = [];
  loading = true;
  pageSize = 10;
  pageIndex = 1;

  constructor(
    private accountService: BankAccountService,
    private location: Location,
    private router: Router,
    private message: NzMessageService,
    private modal: NzModalService
  ) { }

  ngOnInit(): void {
    this.loadDataFromServer(this.pageIndex, this.pageSize, null, null, []);
  }

  loadDataFromServer(
    pageIndex: number,
    pageSize: number,
    sortField: string | null,
    sortOrder: string | null,
    filters: Array<{ key: string; value: string[] }>
  ): void {
    this.loading = true;
    this.accountService
      .getVats(pageIndex, pageSize, sortField, sortOrder, filters)
      .subscribe(
        (response) => {
          this.loading = false;
          this.total = response.totalCount; // Set totalCount for pagination
          this.listOfAccounts = response // Set items array from the response
        },
        (error) => {
          console.error('Error loading VAT data:', error);
          this.loading = false;
        }
      );
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex, sort, filter } = params;
    const currentSort = sort.find((item) => item.value !== null);
    const sortField = currentSort?.key || null;
    const sortOrder = currentSort?.value || null;
    this.loadDataFromServer(pageIndex, pageSize, sortField, sortOrder, filter);
  }

  deleteVat(vat: any): void {
    this.accountService.deleteVat(vat.vatId).subscribe(
      () => {
        this.listOfAccounts = this.listOfAccounts.filter((v) => v.vatId !== vat.vatId);
        this.message.success('VAT deleted successfully.');
        this.loadDataFromServer(this.pageIndex, this.pageSize, null, null, []);
        this.router.navigate(['/vat/all']);
      },
      (error) => {
        this.message.error('Failed to delete VAT.');
      }
    );
  }
  showDeleteConfirm(vat: any): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this VAT?',
      nzContent: '<b style="color: red;">This action cannot be undone.</b>',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => this.deleteVat(vat),
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel')
    });
  }


  // Navigate to the add VAT form
  addVat(): void {
    this.router.navigate(['/vat/add']);
  }

  onBack(): void {
    this.location.back();
  }
}
