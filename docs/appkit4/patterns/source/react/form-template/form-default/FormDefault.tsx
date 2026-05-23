// @ts-nocheck
import React, { useState } from 'react';
import ReactDOM from "react-dom";
import classNames from 'classnames';
import { Input, Checkbox, CalendarPicker, TextArea } from '@appkit4/react-components';
import Form from './Form';
import { useFormValidator } from "./useFormValidator";
import './form.scss';

const FormDefault = (props: {solid?: boolean}) => {

    const [form, setForm] = useState({
        title: "",
        date: undefined,
        description: "",
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

    const onDateChange = (date: Date) => {
        const nextFormState = {
            ...form,
            'date': date,
        };
        setForm(nextFormState);
        if (errors['date'].dirty) {
            validateForm({
                form: nextFormState,
                errors,
                field: 'date',
            });
        }
    }

    const handleOk = (e) => {
        e.preventDefault();
        const { isValid } = validateForm({ form, errors, forceTouchErrors: true });
        console.log(isValid);
        if (!isValid) return;
    }

    return (
        <Form
            solid={props.solid}
            header={
                <>
                    <span className='ap-pattern-form-title'>
                        Report Details
                    </span>
                    <span className='ap-pattern-form-required-indicator'>
                        Required Fields
                    </span>
                </>
            }
            onOk={handleOk}
        >
            <>
                <div className={classNames('ap-pattern-form-item', {
                    'error': errors.title.dirty && errors.title.error
                })}>
                    <Input
                        className={classNames({
                            'error': errors.title.dirty && errors.title.error
                        })}
                        name="title"
                        title={"Title"}
                        errorNode={(<div id="errormessage" aria-live="polite" className="ap-field-validation-error">Please enter a title</div>)}
                        required
                        error={errors.title.dirty && errors.title.error}
                        value={form.title}
                        onChange={onUpdateField}
                    >
                    </Input>
                </div>
                <div className='ap-pattern-form-item'>
                    <CalendarPicker
                        className={classNames({
                            'required-error': errors.date.dirty && errors.date.error
                        })}
                        placeholder="mm/dd/yyyy"
                        locale="en"
                        required
                        error
                        fieldTitle="Select a date"
                        fieldWidth={'100%'}
                        value={form.date}
                        datePanelHorizontalAlign={'right'}
                        onChange={onDateChange}
                    />
                </div>
                <div className='ap-pattern-form-item'>
                    <TextArea
                        name="description"
                        // className={classNames({
                        //     'error': errors.description.dirty && errors.description.error
                        // })}
                        // required
                        title={"Description"}
                        value={form.desc}
                        onChange={onUpdateField}
                    ></TextArea>
                </div>
                <div className='ap-pattern-form-item'>
                    <Checkbox name="rememberMe" checked={form.rememberMe} onChange={onRememberMeChange}>Remember me</Checkbox>
                </div>
            </>
        </Form>
    );
}

export default FormDefault;


class Render {

    static initialize(containerId: string, type: string) {
        ReactDOM.render(<FormDefault />, document.getElementById(containerId));
    }
  }


  export { Render } ;
