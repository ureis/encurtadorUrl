import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService, CreateUrlRequest } from '../../services/url.service';

@Component({
  selector: 'app-url-shortener',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './url-shortener.component.html',
  styleUrl: './url-shortener.component.scss'
})
export class UrlShortenerComponent {
  originalUrl = '';
  alias = '';
  shortUrl: string | null = null;
  loading = false;
  error: string | null = null;

  constructor(private urlService: UrlService) {}

  createShortUrl() {
    if (!this.originalUrl.trim()) {
      this.error = 'Por favor, insira uma URL válida';
      return;
    }

    this.loading = true;
    this.error = null;
    this.shortUrl = null;

    const request: CreateUrlRequest = {
      originalUrl: this.originalUrl,
      alias: this.alias || null
    };

    this.urlService.createShortUrl(request).subscribe({
      next: (response) => {
        this.shortUrl = response.shortUrl;
        this.loading = false;
        this.urlService.notifyUrlsUpdated();
      },
      error: (error) => {
        this.error = error.message;
        this.loading = false;
      }
    });
  }

  copyToClipboard() {
    if (this.shortUrl) {
      navigator.clipboard.writeText(this.shortUrl).then(() => {
        alert('URL copiada para a área de transferência!');
      });
    }
  }

  reset() {
    this.originalUrl = '';
    this.alias = '';
    this.shortUrl = null;
    this.error = null;
  }
}
