import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { VatService } from 'src/app/services/vat.service';

@Component({
  selector: 'app-account-add',
  templateUrl: './account-add.component.html',
  styleUrls: ['./account-add.component.css'],
  standalone: false
})
export class AccountAddComponent {
  addVatForm: FormGroup;

  constructor(
    private fb: NonNullableFormBuilder,
    private vatService: VatService,
    private router: Router,
    private message: NzMessageService
  ) {
    this.addVatForm = this.fb.group({
      percentage: new FormControl<number | null>(null, [Validators.required, Validators.min(0), Validators.max(100)]), // Percentage field
    });
  }

  submitForm(): void {
    if (this.addVatForm.valid) {
      const newVat = {
        ...this.addVatForm.value,
      };

      this.vatService.addVat(newVat).subscribe({
        next: () => {
          this.message.success('VAT added successfully');
          this.router.navigate(['/vat/all']); // Navigate back to VAT list
        },
        error: (err) => {
          this.message.error('Error adding VAT');
          console.error('Error adding VAT:', err);
        },
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.addVatForm.reset();
  }

  onBack(): void {
    this.router.navigate(['/vat/all']); // Navigate back to VAT list
  }
}
