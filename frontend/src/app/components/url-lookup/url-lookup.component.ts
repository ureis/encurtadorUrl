import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlService } from '../../services/url.service';

interface LookupResult {
  shortCode: string;
  originalUrl: string;
  shortUrl: string;
}

@Component({
  selector: 'app-url-lookup',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './url-lookup.component.html',
  styleUrl: './url-lookup.component.scss'
})
export class UrlLookupComponent {
  shortCode = '';
  result: LookupResult | null = null;
  loading = false;
  error: string | null = null;

  constructor(private urlService: UrlService) {}

  lookupUrl() {
    const code = this.shortCode.trim();

    if (!code) {
      this.error = 'Por favor, informe um código curto';
      return;
    }

    this.loading = true;
    this.error = null;
    this.result = null;

    this.urlService.getOriginalUrl(code).subscribe({
      next: (originalUrl) => {
        this.result = {
          shortCode: code,
          originalUrl,
          shortUrl: this.urlService.buildShortUrl(code)
        };
        this.loading = false;
      },
      error: (err) => {
        this.error = err.message;
        this.loading = false;
      }
    });
  }

  reset() {
    this.shortCode = '';
    this.result = null;
    this.error = null;
  }

  copyToClipboard(url: string) {
    navigator.clipboard.writeText(url).then(() => {
      alert('URL copiada para a área de transferência!');
    });
  }

  openShortUrl(shortUrl: string) {
    window.open(shortUrl, '_blank');
  }
}
