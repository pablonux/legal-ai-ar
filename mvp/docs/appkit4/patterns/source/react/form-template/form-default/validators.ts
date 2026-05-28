import { validationMessageLanguages } from './data';

export const titleValidator = (title: string) => {
    if (!title||title.trim()==='') {
      return validationMessageLanguages.titleRequired;
    }
    return "";
  }

  export const dateValidator = (date: string) => {
    if (!date) {
      return validationMessageLanguages.dateRequired;
    }
    return "";
  }
  
  export const descriptionValidator = (description: string) => {
    if (!description||description.trim()==='') {
      return validationMessageLanguages.descriptionRequired;
    }
    return "";
  }


  export const userNameValidator = (email: string) => {
    if (!email||email.trim()==='') {
      return validationMessageLanguages.emailRequired;
    } else if (!new RegExp(/\S+@\S+\.\S+/).test(email)) {
      return validationMessageLanguages.emailValid;
    }
    return "";
  };
  
  export const emailValidator = (email: string) => {
      if (!email||email.trim()==='') {
        return validationMessageLanguages.emailRequired;
      } else if (!new RegExp(/\S+@\S+\.\S+/).test(email)) {
        return validationMessageLanguages.validEmail;
      }
      return "";
    };


    export const firstNameValidator = (name: string) => {
      if (!name||name.trim()==='') {
        return validationMessageLanguages.firstNameRequired;
      }
      return "";
    }

    export const lastNameValidator = (name: string) => {
      if (!name||name.trim()==='') {
        return validationMessageLanguages.lastNameRequired;
      }
      return "";
    }
    
    export const passwordValidator = (password: string) => {
      if (!password||password.trim()==='') {
        return validationMessageLanguages.passwordRequired;
      } 
      // else if (password.length < 8) {
      //   return "Password must have a minimum 8 characters";
      // } 
      else if (!password.match(/[A-Za-z]/g)) {
        return validationMessageLanguages.passwordLetter;
      } else if (!password.match(/[0-9]/g)) {
        return validationMessageLanguages.passwordNumber;
      } else if (!password.match(/[\.\@\$\!\%*\#\_\~\?\&\^]/g)) {
        return validationMessageLanguages.passwordCharacter;
      }
      return "";
    };
    
    export const confirmPasswordValidator = (confirmPassword: string, form: any) => {
      if (!confirmPassword||confirmPassword.trim()==='') {
        return validationMessageLanguages.passwordRequired;
      } else if (confirmPassword !== form.password) {
        return validationMessageLanguages.passwordsNotMatch;
      }

      return "";
    };


          // if (!confirmPassword) {
      //   return "Confirm password is required";
      // } else if (confirmPassword.length < 8) {
      //   return "Confirm password must have a minimum 8 characters";
      // } else if (confirmPassword !== form.password) {
      //   return "Passwords do not match";
      // }