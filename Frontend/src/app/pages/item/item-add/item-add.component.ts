import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ItemService } from 'src/app/services/item.service';

@Component({
    selector: 'app-item-add',
    templateUrl: './item-add.component.html',
    styleUrls: ['./item-add.component.less'],
    standalone: false
})
export class ItemAddComponent {
  addItemForm: FormGroup;

  // Hardcoded user_id for now, will be dynamic later
  private user_id = '9b0e09a7-31d6-4897-8a3e-cc4cf4d1433a';

  constructor(
    private fb: NonNullableFormBuilder,
    private itemService: ItemService,
    private router: Router,
    private message: NzMessageService
  ) {
    this.addItemForm = this.fb.group({
      name: new FormControl<string | null>(null, [Validators.required]),
      description: new FormControl<string | null>(null),
      purchasePrice: new FormControl<number | null>(null, [
        Validators.required,
      ]),
      salePrice: new FormControl<number | null>(null, [Validators.required]),
      quantity: new FormControl<number | null>(null, [Validators.required]),
    });
  }

  submitForm(): void {
    if (this.addItemForm.valid) {
      const newItem = {
        // Generate a new GUID for itemId
        ...this.addItemForm.value,
        user_id: this.user_id, // Hardcoded user_id
      };

      this.itemService.createItem(newItem).subscribe({
        next: () => {
          this.message.success('Item added successfully');
          this.router.navigate(['/items/all']); // Redirect after success
        },
        error: (err) => {
          this.message.error('Error adding item');
          console.error('Error adding item:', err);
        },
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.addItemForm.reset();
  }

  onBack(): void {
    this.router.navigate(['/items/all']);
  }
}
