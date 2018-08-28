import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { InjectionToken, NgModule } from '@angular/core';

import { FingerPrintInterceptor } from '@app/fingerprint/fingerprint.interceptor';

export const FINGERPRINT = new InjectionToken<string>('fingerPrint');

@NgModule({
  imports: [CommonModule],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: FingerPrintInterceptor, multi: true }]
})
export class FingerprintModule {}
