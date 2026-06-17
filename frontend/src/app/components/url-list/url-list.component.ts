import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { UrlListItem, UrlService } from '../../services/url.service';

@Component({
  selector: 'app-url-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './url-list.component.html',
  styleUrl: './url-list.component.scss'
})
export class UrlListComponent implements OnInit, OnDestroy {
  shortUrls: UrlListItem[] = [];
  loading = true;
  error: string | null = null;
  private subscriptions = new Subscription();

  constructor(private urlService: UrlService) {}

  ngOnInit() {
    this.loadUrls();

    this.subscriptions.add(
      this.urlService.urlsUpdated$.subscribe(() => this.loadUrls())
    );
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  loadUrls() {
    this.loading = true;
    this.error = null;

    this.urlService.getAllUrls().subscribe({
      next: (urls) => {
        this.shortUrls = urls;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.message;
        this.shortUrls = [];
        this.loading = false;
      }
    });
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
