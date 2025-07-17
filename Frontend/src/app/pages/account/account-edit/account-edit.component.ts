import { Component, OnInit } from '@angular/core';
import { FormGroup, NonNullableFormBuilder, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BankAccountService } from 'src/app/services/bank-account.service';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.css'],
  standalone: false
})
export class AccountEditComponent implements OnInit {
  editAccountForm: FormGroup;
  accountId!: string;
  listOfBanks: string[] = [];

  constructor(
    private fb: NonNullableFormBuilder,
    private accountService: BankAccountService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService
  ) {
    this.editAccountForm = this.fb.group({
      bankName: new FormControl<string | null>(null, [Validators.required]),
      accountNumber: new FormControl<string | null>(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    this.accountId = this.route.snapshot.paramMap.get('accountId')!;
    this.accountService.getBanks().subscribe((banks) => {
      this.listOfBanks = banks;
    });
    this.accountService.getAccountById(this.accountId).subscribe((account) => {
      this.editAccountForm.patchValue({
        bankName: account.bankName,
        accountNumber: account.accountNumber
      });
    }, error => {
      this.message.error('Error loading Account details');
    });
  }

  submitForm(): void {
    if (this.editAccountForm.valid) {
      const updatedAccount = {
        bankAccountId: this.accountId,
        ...this.editAccountForm.value
      };

      this.accountService.updateAccount(this.accountId, updatedAccount).subscribe({
        next: () => {
          this.message.success('Account updated successfully');
          this.router.navigate(['/bank-accounts/all']);
        },
        error: (err) => {
          this.message.error('Error updating Account');
          console.error('Error updating Account:', err);
        }
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.editAccountForm.reset();
  }

  onBack(): void {
    this.router.navigate(['/bank-accounts/all']);
  }

}
