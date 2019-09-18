import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/User';




@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'users';
  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl);
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + '/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + '/' + id, user);
  }


  setMainPhoto(userId: number, photoId: number) {
    const url = this.baseUrl + '/' + userId + '/photos/' + photoId + '/setMain';
    return this.http.post(url, {});
  }

  deletePhoto(userId: number, photoId: number) {
    const url = this.baseUrl + '/' + userId + '/photos/' + photoId;
    return this.http.delete(url);
  }
}
