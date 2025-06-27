import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateStatusPipe } from '../pipes/translate-status.pipe';

@NgModule({
    declarations: [
        TranslateStatusPipe
    ],
    imports: [
        CommonModule
    ],
    exports: [
        TranslateStatusPipe
    ]
})
export class SharedModule { }