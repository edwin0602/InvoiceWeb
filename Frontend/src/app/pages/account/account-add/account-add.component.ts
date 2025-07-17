import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { BankAccountService } from 'src/app/services/bank-account.service';

@Component({
  selector: 'app-account-add',
  templateUrl: './account-add.component.html',
  styleUrls: ['./account-add.component.css'],
  standalone: false
})
export class AccountAddComponent implements OnInit {
  addAccountForm: FormGroup;
  listOfBanks: string[] = [];

  constructor(
    private fb: NonNullableFormBuilder,
    private accountService: BankAccountService,
    private router: Router,
    private message: NzMessageService
  ) {
    this.addAccountForm = this.fb.group({
      bankName: new FormControl<string | null>(null, [Validators.required]),
      accountNumber: new FormControl<string | null>(null, [Validators.required])
    });
  }
  
  ngOnInit(): void {
    this.accountService.getBanks().subscribe((banks) => {
      this.listOfBanks = banks;
    });
  }

  submitForm(): void {
    if (this.addAccountForm.valid) {
      const newAccount = {
        ...this.addAccountForm.value,
      };

      this.accountService.addAccount(newAccount).subscribe({
        next: () => {
          this.message.success('Account added successfully');
          this.router.navigate(['/bank-accounts/all']);
        },
        error: (err) => {
          this.message.error('Error adding Account');
          console.error('Error adding Account:', err);
        },
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.addAccountForm.reset();
  }

  onBack(): void {
    this.router.navigate(['/bank-accounts/all']);
  }
}
