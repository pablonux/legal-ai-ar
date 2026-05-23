// @ts-nocheck
import React, { useMemo, useState } from 'react';
import ReactDOM from "react-dom";
import classNames from 'classnames';
import { Input, Checkbox, Button } from '@appkit4/react-components';
import { ItemDataType } from '@appkit4/react-components';
import Form from './Form';
import Language from './Language';
import { languages, textLanguages } from './data';
import { useFormValidator } from "./useFormValidator";
import './form.scss';

const FormRegister = (props: {solid?: boolean}) => {
    const forgotPwdLink = 'https://signin.test.access.pwc.com/reset-password?goto=https:%2F%2Fsignin.test.access.pwc.com:443%2Fopenam%2Foauth2%2Fventuressso%2Fauthorize%3Fresponse_type%3Dcode%26redirect_uri%3Dhttps:%2F%2Fappkitdev.pwc.com%2Fappkit4-service%2Fapi%2Fiam%2Faccess%26state%3Dappkit41673542786157%26nonce%3Dtest%26client_id%3Dappkit_dev&realm=%2Fventuressso';
    const [language, setLanguage] = React.useState('en');

    const [form, setForm] = useState({
        firstName: "",
        lastName: "",
        password: "",
        confirmPassword: "",
        rememberMe: true
    });

    const { errors, validateForm, onBlurField } = useFormValidator(form);

    const onUpdateField = (value: string, e: React.ChangeEvent<HTMLInputElement>) => {
        const field = e.target.name;
        const nextFormState = {
            ...form,
            [field]: value,
        };
        setForm(nextFormState);

        if (errors[field].dirty) {
            validateForm({
                form: nextFormState,
                errors,
                field,
            });
        }
    };

    const onRememberMeChange = (checked: boolean) => {
        const nextFormState = {
            ...form,
            'rememberMe': checked,
        };
        setForm(nextFormState);
    }

    const onSelect = (value: string, item: ItemDataType) => {
        setLanguage(value);
    }

    const labels = useMemo(() => {
        return textLanguages[language].value;
    }, [language, textLanguages])


    const handleCreate = (e) => {
        e.preventDefault();
        const { isValid } = validateForm({ form, errors, forceTouchErrors: true });
        console.log(isValid);
        if (!isValid) return;
    }

    return (
        <Form
            solid={props.solid}
            cancelText={labels.loginText}
            okText={labels.ssoLoginText}
            extra={
                <Language
                    value={language}
                    data={languages}
                    onSelect={onSelect}
                ></Language>
            }
            headerLayout={'vertical'}
            header={
                <>
                    <span className='ap-pattern-form-login-welcome'>
                        {labels.title}
                    </span>
                    <div className='ap-pattern-form-login-title'>
                        {labels.name}
                    </div>
                </>
            }
            footer={<Button onClick={handleCreate}>{labels.createAccount}</Button>}
            footerLayout='vertical'
        >
            <>
                <div className='ap-pattern-form-item'>
                    <Input
                        className={classNames({
                            'error': errors.firstName.dirty && errors.firstName.error
                        })}
                        type={"text"}
                        title={labels.firstName}
                        name="firstName"
                        value={form.firstName}
                        onChange={onUpdateField}
                    ></Input>
                </div>
                <div className='ap-pattern-form-item'>
                    <Input
                        className={classNames({
                            'error': errors.lastName.dirty && errors.lastName.error
                        })}
                        type={"text"}
                        title={labels.lastName}
                        name="lastName"
                        value={form.lastName}
                        onChange={onUpdateField}
                    ></Input>
                </div>
                <div className='ap-pattern-form-item'>
                    <div className='ap-field-demo-wrapper ap-field-password'>

                        <Input
                            error = {errors.password.dirty && errors.password.error}
                            type={"password"}
                            title={labels?.password}
                            name="password"
                            value={form.password}
                            onChange={onUpdateField}
                            onBlur={onBlurField}
                        >
                            <div aria-hidden="true" className={classNames("ap-field-password-creator", {
                                'error': errors.password.dirty && errors.password.error
                            })}>
                                <span className={classNames({ "highlight": form.password && form.password.match(/[A-Za-z]/g) })}>
                                    <span className="Appkit4-icon icon-circle-checkmark-fill"></span>
                                    <span className="ap-field-password-condition">{labels?.validLetter}</span>
                                </span>
                                <span className={classNames({ "highlight": form.password && form.password.match(/[0-9]/g) })}>
                                    <span className="Appkit4-icon icon-circle-checkmark-fill"></span>
                                    <span className="ap-field-password-condition">{labels?.validNumber}</span>
                                </span>
                                <span className={classNames({ "highlight": form.password && form.password.match(/[\.\@\$\!\%*\#\_\~\?\&\^]/g) })}>
                                    <span className="Appkit4-icon icon-circle-checkmark-fill"></span>
                                    <span className="ap-field-password-condition">{labels?.validCharacter}</span>
                                </span>
                            </div>
                            <div aria-live="polite" className="ap-field-password-condition-sr-only">
                                <span>{form.password && form.password.match(/[A-Za-z]/g) ? "" : "Not "}contains letter</span>
                                <span >{form.password && form.password.match(/[0-9]/g) ? "" : "Not "}contains number</span>
                                <span>{form.password && form.password.match(/[\.\@\$\!\%\*\#\_\~\?&\^]/g) ? "" : "Not "}contains special character</span>
                            </div>

                        </Input>

                        
                    </div>

                </div>
                <div className='ap-pattern-form-item'>
                    <Input
                        className={classNames({
                            'error': errors.confirmPassword.dirty && errors.confirmPassword.error
                        })}
                        type={"password"}
                        title={labels?.confirmPassword}
                        name="confirmPassword"
                        value={form.confirmPassword}
                        onChange={onUpdateField}
                    >
                        <div className='ap-field-password-error'>
                            {errors.confirmPassword.error && form.confirmPassword && errors.confirmPassword.message[language]}
                        </div>
                    </Input>
                </div>
                <div className='ap-pattern-form-item remember-me'>
                    <Checkbox name="rememberMe" checked={form.rememberMe} onChange={onRememberMeChange}>{labels.rememberMe}</Checkbox>
                    <span>
                        <a href={forgotPwdLink} target="_blank">{labels?.forgotPassword}</a>
                    </span>
                </div>
            </>
        </Form>
    );
}

export default FormRegister;

class Render {

    static initialize(containerId: string, type: string) {
        // const Component = AccordionComponent(multiple, show);

        ReactDOM.render(<FormRegister />, document.getElementById(containerId));
    }
  }


  export { Render } ;