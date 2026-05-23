
// @ts-nocheck
import { useState } from "react";
import {
  emailValidator,
  passwordValidator,
  confirmPasswordValidator,
  titleValidator,
  descriptionValidator,
  firstNameValidator,
  lastNameValidator,
  userNameValidator,
  dateValidator
} from "./validators";

const touchErrors = errors => {
  return Object.entries(errors).reduce((acc, [field, fieldError]) => {
    acc[field] = {
      ...fieldError,
      dirty: true,
    };
    return acc;
  }, {});
};

export const useFormValidator = form => {
    const [errors, setErrors] = useState({
      title: {
        dirty: false,
        error: false,
        message: "",
      },
      description: {
        dirty: false,
        error: false,
        message: "",
      },
      date: {
        dirty: false,
        error: false,
        message: "",
      },
      email: {
        dirty: false,
        error: false,
        message: "",
      },
      firstName: {
        dirty: false,
        error: false,
        message: "",
      },
      lastName: {
        dirty: false,
        error: false,
        message: "",
      },
      userName: {
        dirty: false,
        error: false,
        message: "",
      },
      password: {
        dirty: false,
        error: false,
        message: "",
      },
      confirmPassword: {
        dirty: false,
        error: false,
        message: "",
      },
    });
  
    const validateForm = ({ form, field, errors, forceTouchErrors = false }, isOnBlur = false) => {
      
      let isValid = true;
  
      // Create a deep copy of the errors
      let nextErrors = JSON.parse(JSON.stringify(errors));
  
      // Force validate all the fields
      if (forceTouchErrors) {
        nextErrors = touchErrors(errors);
      }
  
      const { email, password, confirmPassword, title, description, firstName, lastName, userName, date } = form;

      if (form.hasOwnProperty('title') && nextErrors.title.dirty && (field ? field === "title" : true)) {
        const titleMessage = titleValidator(title, form);
        nextErrors.title.error = !!titleMessage;
        nextErrors.title.message = titleMessage;
        if (!!titleMessage) isValid = false;
      }

      if (form.hasOwnProperty('date') && nextErrors.date.dirty && (field ? field === "date" : true)) {
        const dateMessage = dateValidator(date, form);
        nextErrors.date.error = !!dateMessage;
        nextErrors.date.message = dateMessage;
        if (!!dateMessage) isValid = false;
      }

      // if (form.hasOwnProperty('description') && nextErrors.description.dirty && (field ? field === "description" : true)) {
      //   const descriptionMessage = descriptionValidator(description, form);
      //   nextErrors.description.error = !!descriptionMessage;
      //   nextErrors.description.message = descriptionMessage;
      //   if (!!descriptionMessage) isValid = false;
      // }
  
      if (form.hasOwnProperty('email') && nextErrors.email.dirty && (field ? field === "email" : true)) {
        const emailMessage = emailValidator(email, form);
        nextErrors.email.error = !!emailMessage;
        nextErrors.email.message = emailMessage;
        if (!!emailMessage) isValid = false;
      }
  
      if (form.hasOwnProperty('firstName') && nextErrors.firstName.dirty && (field ? field === "firstName" : true)) {
        const firstNameMessage = firstNameValidator(firstName, form);
        nextErrors.firstName.error = !!firstNameMessage;
        nextErrors.firstName.message = firstNameMessage;
        if (!!firstNameMessage) isValid = false;
      }

      if (form.hasOwnProperty('lastName') && nextErrors.lastName.dirty && (field ? field === "lastName" : true)) {
        const lastNameMessage = lastNameValidator(lastName, form);
        nextErrors.lastName.error = !!lastNameMessage;
        nextErrors.lastName.message = lastNameMessage;
        if (!!lastNameMessage) isValid = false;
      }

      if (form.hasOwnProperty('userName') && nextErrors.userName.dirty && (field ? field === "userName" : true)) {
        const userNameMessage = userNameValidator(userName, form);
        nextErrors.userName.error = !!userNameMessage;
        nextErrors.userName.message = userNameMessage;
        if (!!userNameMessage) isValid = false;
      }

      if (form.hasOwnProperty('password') && nextErrors.password.dirty && (field ? field === "password" : true)) {
        let passwordMessage = passwordValidator(password, form);
        if(isOnBlur&&password==='') passwordMessage = '';
        nextErrors.password.error = !!passwordMessage;
        nextErrors.password.message = passwordMessage;
        if (!!passwordMessage) isValid = false;
      }
  
      if (form.hasOwnProperty('confirmPassword') && 
        nextErrors.confirmPassword.dirty &&
        (field ? field === "confirmPassword" : true)
      ) {
        const confirmPasswordMessage = confirmPasswordValidator(
          confirmPassword,
          form
        );
        nextErrors.confirmPassword.error = !!confirmPasswordMessage;
        nextErrors.confirmPassword.message = confirmPasswordMessage;
        if (!!confirmPasswordMessage) isValid = false;
      }
  
      setErrors(nextErrors);
  
      return {
        isValid,
        errors: nextErrors,
      };
    };
  
    const onBlurField = e => {
      const field = e.target.name;
      const fieldError = errors[field];
      if (fieldError.dirty) return;
  
      const updatedErrors = {
        ...errors,
        [field]: {
          ...errors[field],
          dirty: true,
        },
      };
  
      validateForm({ form, field, errors: updatedErrors }, true);
    };
  
    return {
      validateForm,
      onBlurField,
      errors,
    };
  };