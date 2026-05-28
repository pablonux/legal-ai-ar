import React from 'react';
import { Breadcrumb, BreadcrumbItem, Button, Tooltip } from '@appkit4/react-components';
import classNames from 'classnames';
import './header-with-back.scss';

export interface HeaderWithBackProps {
    onClickEvent?: (event: React.MouseEvent<HTMLElement>|null) => void;
    onClickBreadCrumbItem?: (index: number) => void;
    onKeyDownBreadCrumbItem?: (index: number) => void;
    className?: string;
    tooltipContent?: string, 
    breadcrumbList?: any[],
    disabledBack?: boolean,
    type?: string,
    withBreadcrumbs?: boolean,
    title?: string;
    content?: string;
}

const PageHeaderWithBackButton = React.forwardRef<HTMLElement, HeaderWithBackProps>((props: HeaderWithBackProps, ref) => {
    const {
        onClickEvent,
        onClickBreadCrumbItem,
        className,
        tooltipContent = 'Back',
        breadcrumbList = [{
            name: 'Product',
            link: 'Product'
          },{
              name: 'Parent page',
              link: 'Parent'
          },{
              name: 'This page',
              link: 'This'
          }],
        disabledBack = false,
        type = 'small',
        withBreadcrumbs = true,
        title = 'Lorem heading describing content to expect on the page',
        content = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Faucibus et molestie ac feugiat sed lectus vestibulum mattis ullamcorper. Lorem donec massa sapien faucibus et molestie ac feugiat sed.'
    } = props;

    const onKeyDownItem = (event: React.KeyboardEvent<HTMLElement>, value: number) => {
        const { key } = event;
        if (key === 'Enter') {
            onClickBreadCrumbItem?.(value);
        }
    }

    const onKeyDownBack = (event: React.KeyboardEvent<HTMLElement>, value: number) => {
        event.stopPropagation();
        event.preventDefault();
        const { key } = event;
        if (key === 'Enter') {
            onClickEvent?.(null);
        }
    }

    return (
        <div className={classNames('ap-pattern-header-with-back', className, {
            '': type === 'small',
            'medium': type === 'medium',
            'large': type === 'large'
        })}>
            {withBreadcrumbs && <div className='ap-pattern-header-bread-crumb'>
                <Breadcrumb>
                {
                    breadcrumbList?.map((item: any, index: number) => {
                        return <BreadcrumbItem key={index} onClick={() => onClickBreadCrumbItem?.(index)} onKeyDown={(event: any) => onKeyDownItem?.(event, index)}>
                            <span tabIndex={index === breadcrumbList.length - 1 ? -1 : 0}>{item.name}</span>
                        </BreadcrumbItem>
                    })
                }
                </Breadcrumb>
            </div>}
            <div className={classNames("header-with-back-content", {
                'without-breadcrumbs': !withBreadcrumbs
            })}>
                <div className="header-with-back-button-title">
                    <Tooltip disabled={disabledBack} trigger='hover' content={tooltipContent} distance={8} position={'top'} target={`.ap-pattern-header-btn-back`}></Tooltip>
                    <Button className="ap-pattern-header-btn-back" disabled={disabledBack} kind='tertiary' icon="icon-arrow-left-outline" onClick={onClickEvent} onKeyDown={onKeyDownBack}></Button>
                    <h1 className='header-title'>{title}</h1>
                </div>
                <h2 className="header-with-back-button-detail">
                    {content}
                </h2>
            </div>
        </div>
    )
})

export default PageHeaderWithBackButton