import React, { useMemo } from 'react';
// import ReactDOM from "react-dom";
import { Input, CalendarPicker, Checkbox, Select, TextArea, ItemDataType } from '@appkit4/react-components';
import Form from './Form';
import Language from './Language';
import { languages, textLanguages } from './data';
import { useFormValidator } from "./useFormValidator";
import './form.scss';
import { validationMessageLanguages } from './data';

const FormCol2 = (props: {solid?: boolean}) => {

    const [language, setLanguage] = React.useState('en');

    const DropdownSample = (categoryName?: string) => {
        const [value, setValue] = React.useState('');
        const data = [
            { value: 'item1', label: 'Default' },
            { value: 'item2', label: 'Disabled' },
            { value: 'item3', label: 'Icon', iconName: 'thumb-up-outline' },
            { value: 'item4', label: 'Badge', badgeValue: 'New' },
            { value: 'item5', label: 'Description', descValue: 'Lorem ipsum' },
        ];

        return (
            <Select
                data={data}
                value={value}
                searchable={false}
                onSelect={(vals: any) => setValue(vals)}
                placeholder={categoryName}
            >
            </Select>
        );
    }

    const onSelect = (value: string, item: ItemDataType) => {
        setLanguage(value);
    }

    const labels = useMemo(() => {
        return textLanguages[language].value;
    }, [language, textLanguages])

    return (
        <Form
            headerLayout={'vertical'}
            okText={labels.submitText}
            cancelText={labels.cancelText}
            extra={
                <Language
                    value={language}
                    data={languages}
                    onSelect={onSelect}
                ></Language>
            }
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
        >
            <div className='ap-pattern-form-item'>
                <div className="ap-container">
                    <div className="row">
                        <div className="col">
                            <Input
                                type={"text"}
                                title={labels.titleLabel}
                            >
                            </Input>
                        </div>
                        <div className="col">
                            {DropdownSample(labels.category)}
                        </div>
                    </div>
                </div>
            </div>
            <div className='ap-pattern-form-item'>
                <div className="ap-container">
                    <div className="row">
                        <div className="col">
                            {DropdownSample(labels.subCategory)}
                        </div>
                        <div className="col">
                            <CalendarPicker
                                placeholder="mm/dd/yyyy"
                                locale="en"
                                fieldTitle={labels.dueDate}
                                // @ts-ignore
                                fieldWidth={'100%'}
                                customErrorNode={(<div id="errormessage" aria-live="polite" className="ap-calendar-validation-error">{(validationMessageLanguages.validDate as any)[language]}</div>)}
                            />
                        </div>
                    </div>
                </div>
            </div>

            <div className='ap-pattern-form-item'>
                <TextArea title={labels.detailsLabel}></TextArea>
            </div>

            <div className='ap-pattern-form-item'>
                <Checkbox defaultChecked>{labels.rememberMe}</Checkbox>
            </div>
        </Form>
    )
}

export default FormCol2;


// class Render {

//     static initialize(containerId: string, type: string) {
//         ReactDOM.render(<FormCol2 />, document.getElementById(containerId));
//     }
//   }

//   export { Render } ;
