import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'translate'
})
export class TranslateStatusPipe implements PipeTransform {
  transform(value: string): string {
    switch (value) {
      case 'Final':
        return 'Factura';
      case 'Quotation':
        return 'Cotización';
      case 'Draft':
        return 'Borrador';
      case 'Active':
        return 'Activo';
      case 'Deleted':
        return 'Eliminado';
      case 'Created':
        return 'Creada';
      case 'Cancelled':
        return 'Cancelada';
      case 'Paid':
        return 'Pagada';
      case 'Pending':
        return 'Pendiente';
      case 'PartiallyPaid':
        return 'Parcialmente Pagada';
      case 'Overdue':
        return 'Vencida';
      default:
        return value;
    }
  }
}