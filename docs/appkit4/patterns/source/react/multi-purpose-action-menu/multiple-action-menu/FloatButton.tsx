import React, { MouseEvent, KeyboardEvent } from 'react';
import { Button, ButtonProps, Tooltip } from '@appkit4/react-components';
import classNames from 'classnames';
import './float-button.scss';

export type FloatButtonElement = HTMLAnchorElement & HTMLButtonElement;

export type RenderFunction = () => React.ReactNode;

interface FloatButtonProps extends React.DOMAttributes<FloatButtonElement>{
    className?: string;
    style?: React.CSSProperties;
    icon?: string;
    tooltip?: React.ReactNode | RenderFunction;
    kind?: 'secondary' | 'tertiary' | 'negative' | 'text' | 'primary';
    type?: 'default' | 'inverse' | 'rose';
    add?: boolean;
    htmlType?: 'submit' | 'reset' | 'button';
    "aria-label"?: string;
    onClick?: (event: MouseEvent<HTMLButtonElement> | KeyboardEvent<HTMLButtonElement>) => void; 
}


const FloatButton = React.forwardRef<HTMLButtonElement, FloatButtonProps>((props, ref) => {

    const {
        className,
        tooltip,
        icon,
        kind,
        type = 'default',
        add = true,
        htmlType,
        onClick,
        ...restProps
    } = props;

    const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        onClick?.(event);
    }
    
    const handleKeyDown = (event: React.KeyboardEvent<HTMLButtonElement>) => {
        if (event.key === 'Enter' || event.key === ' ') {
          event.preventDefault();
          onClick?.(event);
        }
      };

    let buttonNode = (
        <button
            ref={ref}
            className={classNames('ap-float-btn', { [`ap-float-btn-${type}`]: type }, className)}
            {...restProps}
            onClick={handleClick}
            onKeyDown={handleKeyDown}
        >
            { icon === 'icon-need-heep-bot-outline' ? (
                <span className={`Appkit4-icon ${icon}`}>
                    <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M2.13949 14.583C1.74515 14.583 1.49345 14.33 1.49345 13.9336V11.3021H0.654431C0.268485 11.3021 0 11.0322 0 10.6442C0 10.2562 0.268485 9.98629 0.654431 9.98629H1.49345V8.65366H0.654431C0.268485 8.65366 0 8.38376 0 7.99578C0 7.59937 0.268485 7.3379 0.654431 7.3379H1.49345V6.00527H0.654431C0.268485 6.00527 0 5.73537 0 5.34739C0 4.95098 0.268485 4.68108 0.654431 4.68108H1.49345V2.06642C1.49345 1.67001 1.74515 1.40854 2.13949 1.40854H4.74882V0.657881C4.74882 0.2699 5.0173 0 5.40325 0C5.79759 0 6.06607 0.2699 6.06607 0.657881V1.40854H7.38333V0.657881C7.38333 0.2699 7.65181 0 8.04615 0C8.43209 0 8.70058 0.2699 8.70058 0.657881V1.40854H10.0178V0.657881C10.0178 0.2699 10.2863 0 10.6807 0C11.0666 0 11.3351 0.2699 11.3351 0.657881V1.40854H13.9444C14.3388 1.40854 14.5905 1.67001 14.5905 2.06642V4.68108H15.3456C15.7315 4.68108 16 4.94254 16 5.33896C16 5.72694 15.7315 5.99684 15.3456 5.99684H14.5905V7.32947H15.3456C15.7315 7.32947 16 7.59937 16 7.98735C16 8.37533 15.7315 8.64523 15.3456 8.64523H14.5905V9.97786H15.3456C15.7315 9.97786 16 10.2478 16 10.6357C16 11.0237 15.7315 11.2936 15.3456 11.2936H14.5905V13.9336C14.5905 14.33 14.3388 14.583 13.9444 14.583H11.3519V15.3337C11.3519 15.7301 11.0918 16 10.6974 16C10.3115 16 10.043 15.7301 10.043 15.3337V14.583H8.70058V15.3337C8.70058 15.7301 8.43209 16 8.04615 16C7.65181 16 7.38333 15.7301 7.38333 15.3337V14.583H6.06607V15.3337C6.06607 15.7301 5.79759 16 5.40325 16C5.0173 16 4.74882 15.7301 4.74882 15.3337V14.583H2.13949ZM2.66667 13.3333H13.3333V2.66667H2.66667V13.3333Z" fill="white"/>
                    <path fillRule="evenodd" clipRule="evenodd" d="M7.68336 11.7112C7.71352 11.8836 7.84276 12 8.00215 12C8.16155 12 8.28648 11.8836 8.31664 11.7112C8.6699 8.99569 9.05331 8.60776 11.7027 8.31466C11.8751 8.29741 12 8.1681 12 8C12 7.8319 11.8751 7.70259 11.7027 7.68103C9.05331 7.38793 8.67421 7.00431 8.31664 4.28448C8.28648 4.11638 8.16155 4 8.00215 4C7.84276 4 7.71352 4.11638 7.68336 4.28448C7.3301 7.00431 6.951 7.39224 4.30156 7.68103C4.12493 7.70259 4 7.8319 4 8C4 8.1681 4.12493 8.2931 4.30156 8.31466C6.951 8.6681 7.30856 9 7.68336 11.7112Z" fill="white"/>
                    </svg>
                </span>
            ) : <span className={`Appkit4-icon ${icon}`}></span>}
            
        </button>
    )

    if ('tooltip' in props) {
        buttonNode = (
            //@ts-ignore
          <Tooltip position="left" content={tooltip}>
            {buttonNode}
          </Tooltip>
        );
    }

    return (
        <>
            {buttonNode}
        </>
    );
})

export default FloatButton;