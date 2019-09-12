import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/User.service';
import { AlertifyService } from '../../services/alertify.service';
import { User } from '../../models/User';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  constructor(private userService: UserService, alertifyService: AlertifyService) { }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers().subscribe((users: User[]) => {
      this.users = users;
    });
  }
}
