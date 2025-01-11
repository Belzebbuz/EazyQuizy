import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  get isDark() {
    return localStorage.getItem('theme') == 'dark'
  }
  constructor() {
    this.setInitialTheme();
  }
  private setInitialTheme() {
    const theme = localStorage.getItem('theme') || 'light';
    this.applyTheme(theme);
  }

  applyTheme(theme: string) {
    if (theme === 'dark') {
      document.documentElement.classList.add('dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('theme', 'light');
    }
  }

  toggleTheme() {
    const currentTheme = localStorage.getItem('theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    this.applyTheme(newTheme);
  }
}
