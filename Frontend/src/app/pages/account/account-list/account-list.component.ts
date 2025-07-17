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
      .getAccounts(pageIndex, pageSize, sortField, sortOrder, filters)
      .subscribe(
        (response) => {
          this.loading = false;
          this.total = response.totalCount;
          this.listOfAccounts = response
        },
        (error) => {
          console.error('Error loading Accounts data:', error);
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

  deleteAccount(account: any): void {
    this.accountService.deleteAccount(account.bankAccountId).subscribe(
      () => {
        this.listOfAccounts = this.listOfAccounts.filter((v) => v.bankAccountId !== account.bankAccountId);
        this.message.success('Account deleted successfully.');
        this.loadDataFromServer(this.pageIndex, this.pageSize, null, null, []);
        this.router.navigate(['/bank-accounts/all']);
      },
      (error) => {
        this.message.error('Failed to delete Account.');
      }
    );
  }

  showDeleteConfirm(account: any): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this Account?',
      nzContent: '<b style="color: red;">This action cannot be undone.</b>',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => this.deleteAccount(account),
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel')
    });
  }

  addAccount(): void {
    this.router.navigate(['/bank-accounts/add']);
  }

  onBack(): void {
    this.location.back();
  }
}
