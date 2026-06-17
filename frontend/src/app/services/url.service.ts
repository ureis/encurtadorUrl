import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, Subject, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface CreateUrlRequest {
  originalUrl: string;
  alias?: string | null;
}

export interface CreateUrlResponse {
  shortUrl: string;
}

export interface UrlListItem {
  shortCode: string;
  originalUrl: string;
  createdAt: string;
  shortUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private apiUrl = 'http://localhost:8080';
  private urlsUpdatedSubject = new Subject<void>();
  urlsUpdated$ = this.urlsUpdatedSubject.asObservable();

  constructor(private http: HttpClient) {}

  createShortUrl(request: CreateUrlRequest): Observable<CreateUrlResponse> {
    return this.http.post<CreateUrlResponse>(`${this.apiUrl}/api/urls`, request)
      .pipe(
        catchError(this.handleError)
      );
  }

  getAllUrls(): Observable<UrlListItem[]> {
    return this.http.get<UrlListItem[]>(`${this.apiUrl}/api/urls`)
      .pipe(
        catchError(this.handleError)
      );
  }

  getOriginalUrl(shortCode: string): Observable<string> {
    return this.http.get(`${this.apiUrl}/api/urls/${shortCode}`, { responseType: 'text' })
      .pipe(
        catchError(this.handleError)
      );
  }

  buildShortUrl(shortCode: string): string {
    return `${this.apiUrl}/${shortCode}`;
  }

  notifyUrlsUpdated(): void {
    this.urlsUpdatedSubject.next();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Erro desconhecido';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro: ${error.error.message}`;
    } else {
      errorMessage = `Erro do servidor: ${error.status} - ${error.statusText}`;
      if (error.error && typeof error.error === 'string') {
        errorMessage = error.error;
      }
    }

    return throwError(() => new Error(errorMessage));
  }
}
