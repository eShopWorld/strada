import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';

import { from, Observable } from 'rxjs';
import { map, mergeMap, tap } from 'rxjs/operators';

import { AppEndPoints } from '@app/core';

import { fingerprint } from './fingerprint';

import * as shajs from 'sha.js';

export class FingerPrintInterceptor implements HttpInterceptor {
  private fingerprintCache: string;

  constructor(private endPoints: AppEndPoints) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.startsWith(this.endPoints.API_ENDPOINT)) {
      return from(this.getMyFingerprintAndCacheIt()).pipe(
        map(fp =>
          req.clone({
            headers: req.headers.set('correlationid', fp)
          })
        ),
        mergeMap(request =>
          next.handle(request).pipe(
            tap((event: HttpEvent<any>) => {
              return next.handle(req);
            })
          )
        )
      );
    }
    return next.handle(req);
  }

  /**
   *  Gets the finger print value from lib and cache it in @property(fingerprint)
   * If there is a cache present, uses the cached value and dont get it from the lib
   */
  private getMyFingerprintAndCacheIt(): Promise < any > {
    if (this.fingerprintCache) {
      return new Promise((resolve) => {
        resolve(this.fingerprintCache);
      });
    }
    return new Promise((resolve) => {
      fingerprint.generateFingerprint().then(uuid => {
        this.fingerprintCache =  shajs('sha256').update(JSON.stringify(uuid)).digest('hex');
        resolve(this.fingerprintCache);
      });
    });
  }
}
