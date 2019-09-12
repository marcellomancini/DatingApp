import { Photo } from './Photo';

export interface User{
    id: number;
    username: string;
    gender: string;
    age: number;
    knownAs: string;
    created: Date;
    lastactive: Date;
    city: string;
    country: string;
    photosurl: string;
    intrests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];

}
