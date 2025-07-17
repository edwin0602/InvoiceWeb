/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { BankAccountService } from './bank-account.service';

describe('Service: BankAccount', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BankAccountService]
    });
  });

  it('should ...', inject([BankAccountService], (service: BankAccountService) => {
    expect(service).toBeTruthy();
  }));
});
