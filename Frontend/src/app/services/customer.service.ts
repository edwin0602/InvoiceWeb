import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private apiUrl = environment.apiUrl + 'customers';

  constructor(private http: HttpClient) { }

  getCustomers(
    pageNumber: number,
    pageSize: number,
    sortField: string | null,
    sortOrder: string | null,
    filters: Array<{ key: string; value: string[] }>,
    searchTerm: string | null
  ): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', `${pageNumber}`)
      .set('pageSize', `${pageSize}`)
      .set('sortField', sortField || '')
      .set('sortOrder', sortOrder || '');
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    filters.forEach((filter) => {
      filter.value.forEach((value) => {
        params = params.append(filter.key, value);
      });
    });

    return this.http
      .get<any>(this.apiUrl, { params })
      .pipe(catchError(() => of([])));
  }

  addCustomer(customer: any): Observable<void> {
    return this.http.post<void>(this.apiUrl, customer);
  }

  updateCustomer(customerId: string, customer: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${customerId}`, customer);
  }

  getCustomerById(customerId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${customerId}`);
  }

  deleteCustomer(customerId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${customerId}`);
  }

  getCustomerStatus(): Observable<any[]> {
    return of([
      { value: 'Active', name: 'Activo' },
      { value: 'Deleted', name: 'Eliminado' }
    ]);
  }

  deleteCustomerFile(customerId: string, fileId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${customerId}/files/${fileId}`);
  }

  uploadCustomerFile(customerId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('fileType', 'support');

    return this.http.post<any>(`${this.apiUrl}/${customerId}/files`, formData);
  }
}
