//@ts-nocheck
import React from 'react';
import classNames from 'classnames';
import { Button, ButtonProps } from '@appkit4/react-components';
import './form.scss';

export interface FormProps {
    className?: string;
    solid?: boolean;
    title?: React.ReactNode;
    extra?: React.ReactNode;
    header?: React.ReactNode;
    footer?: React.ReactNode;
    cancelText?: string;
    okText?: string;
    okButtonProps?: ButtonProps;
    cancelButtonProps?: ButtonProps;
    [key: string]: any;
    onCancel?: () => void;
    onOK?: () => void;
    extra?: React.ReactNode;
    divider?: boolean;
    cancelBtnKind?: string;
    okBtnKind?: string;
    headerLayout?: 'horizontal' | 'vertical';
    footerLayout?: 'horizontal' | 'vertical';
}

export interface FormLocale {
    okText: string;
    cancelText: string;
  }

export const handleCancel = (onCancelFunc) => {
    onCancelFunc?.();
}

export const handleOk = (onOkFunc, e: any) => {
    onOkFunc?.(e);
}

const Form = React.forwardRef<HTMLElement, FormProps>((props, ref) => {

    const {
        className,
        solid = false,
        title,
        extra,
        header,
        body,
        footer,
        cancelText = 'Cancel',
        okText = 'Submit',
        onCancel,
        onOk,
        headerLayout,
        footerLayout,
        divider = false,
        cancelBtnKind = 'secondary',
        okBtnKind = 'primary',
        ...restProps
    } = props;

    const renderFooter = () => {
        return (
            <>
                <Button kind={cancelBtnKind} onClick={()=>handleCancel(onCancel)} {...props.cancelButtonProps}>{cancelText}</Button>
                {divider && <hr />}
                <Button kind={okBtnKind} onClick={(e)=>handleOk(onOk, e)} {...props.okButtonProps}>{okText}</Button>
            </>
        );
    }

    return (
        <div className={classNames('ap-pattern-form', className, { 'solid-form': solid })} {...restProps}>
            <div className={classNames('ap-pattern-form-header', { 'vertical': headerLayout === 'vertical' })}>
                {header}
                {
                    extra ? (
                        <div className='ap-pattern-form-header-extra'>{extra}</div>
                    ) : null
                }
            </div>
            <form className='ap-pattern-form-body'>
                {props.children}
                <div className={classNames('ap-pattern-form-footer', { 'vertical': footerLayout === 'vertical' })}>
                    {
                        footer === undefined ? renderFooter() : footer
                    }
                </div>
            </form>
        </div>
    )
})

export default Form;
