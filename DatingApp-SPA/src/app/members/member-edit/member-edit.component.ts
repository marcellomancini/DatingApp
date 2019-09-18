import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/models/User';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/services/User.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm', { static: true }) editForm: NgForm;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
  photoUrl: string;
  user: User;

  constructor(private route: ActivatedRoute,
    private alertifyService: AlertifyService,
    private userService: UserService,
    private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // tslint:disable-next-line: no-string-literal
      this.user = data['user'];
      this.authService.currentPhotoUrl.subscribe(url => this.photoUrl = url);
    });
  }

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user)
      .subscribe(next => {
        this.alertifyService.success('Profile updated successfully');
        this.editForm.reset(this.user);
      },
        error => {
          this.alertifyService.error(error);
        });
  }

  updateMainPhoto(photoUrl: string) {
    this.user.photoUrl = photoUrl;
  }


}
