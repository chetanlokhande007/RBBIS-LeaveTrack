import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { HeroComponent } from './hero/hero.component';
import { FeaturesComponent } from './features/features.component';
import { HowItWorksComponent } from './how-it-works/how-it-works.component';
import { BenefitsComponent } from './benefits/benefits.component';
import { FaqComponent } from './faq/faq.component';
import { ContactComponent } from './contact/contact.component';
import { FooterComponent } from './footer/footer.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule, 
    HeaderComponent, 
    HeroComponent, 
    FeaturesComponent,
    HowItWorksComponent, 
    BenefitsComponent, 
    FaqComponent, 
    ContactComponent, 
    FooterComponent
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent { }
