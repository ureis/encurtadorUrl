import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { UrlShortenerComponent } from './components/url-shortener/url-shortener.component';
import { UrlLookupComponent } from './components/url-lookup/url-lookup.component';
import { UrlListComponent } from './components/url-list/url-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    UrlShortenerComponent,
    UrlLookupComponent,
    UrlListComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Encurtador de URLs';
}
