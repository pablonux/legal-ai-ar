import React from 'react';
import { Avatar, Button, Footer, Header, HeaderOptionItem, ItemDataType, Navigation, NavigationItem, NavigationProps, HeaderProps } from '@appkit4/react-components';
import './error-page.scss';
import classNames from 'classnames';
export interface ErrorPageProps {
    errorImage?: string;
    onClickBack?:(event: React.MouseEvent<HTMLButtonElement> | React.KeyboardEvent<HTMLButtonElement>) => void,
    onClickLink?:(event: React.MouseEvent<HTMLElement>) => void,
    onSelect?: (value: string, item: ItemDataType) => void,
    pageTitle?: string;
    pageDescription?: string;
    linkText?: string;
    buttonText?: string;
    footerContent?: string;
    footerLinks?: ({
        name: string;
        href: string;
        target: string;
    } | {
        name: string;
        href: string;
        target?: undefined;
    })[];
    linkHref?: string;
    headerProps?:HeaderProps;
    navProps?:NavigationProps
}
export const headerTitle = () => "Appkit";
export const headerContent = () => <HeaderOptionItem iconName="search-outline" label="Search"></HeaderOptionItem>;
export const headerOptions = () => {
    return (
    <>
    <HeaderOptionItem iconName="help-question-outline" label="Support"></HeaderOptionItem>
    <HeaderOptionItem iconName="notification-outline" label="Alerts"></HeaderOptionItem>
    </>
    );
}
export const headerUser = () => (<Avatar label="CD" role="button" disabled={false}></Avatar>);
export const navigationUser = () => <Avatar className="keyboard-focus" label="VR" disabled={false} role="button"></Avatar>;
export const FooterComponent = (content: string, links: {name: string, href: string, target?: string}[]) => {
    return (
        <Footer content={content} type={'links'} links={links}></Footer>
    );
}
const ErrorPage = React.forwardRef<HTMLElement, ErrorPageProps>((props: ErrorPageProps, ref) => {
    const [collapsedVal, setCollapsedVal] = React.useState(false);
    const {
        errorImage,
        onClickBack,
        onClickLink,
        pageTitle = 'Page not found',
        pageDescription = 'We could not find the page you were looking for. Please navigate back or browse our sitemap for help: ',
        linkText = 'View Directory',
        buttonText = 'Go back',
        footerContent = "© 2025 PwC US. All rights reserved. PwC US refers to the US group of member firms and may sometimes refer to the PwC network. Each member firm is a separate legal entity.",
        footerLinks = [
            { name: 'Privacy policy', href: '#', target: '_blank' },
            { name: 'Cookie notice', href: '#', target: '_self'  },
            { name: 'Terms and conditions', href: '#'},
            { name: 'Customize cookie settings', href: '#' }
        ],
        linkHref='',
        headerProps,
        navProps
    } = props; 
    
    const headerTemplate = () => {
        return (
        <div className="header-wrapper">
        <Header
                type="transparent"
                titleTemplate={headerTitle} 
                optionsTemplate={headerOptions}
                contentTemplate={headerContent}
                userTemplate={headerUser}
                {...headerProps}
                >
                </Header>
                </div>
            );
    }
    const onCollapseEvent = (collapsed: boolean, event: React.MouseEvent<HTMLElement> | React.KeyboardEvent<HTMLElement>) => {
        setCollapsedVal(collapsed);
    }

    const navigationTemplate = () => {
           
        const navList: NavigationItem[] = [
            {
                name: 'Welcome',
                prefixIcon: 'hand-wave'
            },
            {
                name: 'Getting started',
                prefixIcon: 'download-cloud'
            },
            {
                name: 'Styleguide',
                prefixIcon: 'venn-abc'
            },
            {
                name: 'Components',
                prefixIcon: 'particulates'
            },
            {
                name: 'Support',
                prefixIcon: 'help-question'
            }
            // {
            //     name: 'Support',
            //     prefixIcon: 'help-question',
            //     suffixIcon: 'down-chevron',
            //     children: [{
            //         name: 'FAQs'
            //     },
            //     {
            //         name: 'Versions'
            //     }]
            // }
        ];
    
        return (  
            <div className="ap-navigation-wrapper">
                <Navigation
                    width={280}
                    className="sitedemo"
                    hasHeader={false}
                    showTooltip={true}
                    navList={navList}
                    selectedIndex={0}
                    onClickCollapseEvent={onCollapseEvent}
                    titleTemplate={headerTitle}
                    userTemplate={navigationUser}
                    collapseBtnText="Collapse"
                    {...navProps}
                    >
                </Navigation>
            </div>
        );
    
    }
    


    return (
        <div className="ap-pattern-error-page">
        {headerTemplate()}
            <div className={classNames("ap-pattern-error-page-content", {
                'nav-collapse': collapsedVal
            })}>
                {navigationTemplate()}
                {/* <div className='page-logo'></div> */}
                <div className="error-page">
                    <div className="div-wrapper">
                        <img alt="" src={errorImage} className="error-image" />
                        <h1 className="ap-pattern-error-page-title">{pageTitle}</h1>
                        <span className="error-text">{pageDescription}</span>
                        <div className='view-directory'>
                            <div className="view-directory-link">
                                <a href={linkHref} target="_blank" className="view-directory-icon" onClick={onClickLink}>
                                    <span className="Appkit4-icon icon-globe-outline"></span><span className="view-directory-text">{linkText}</span>
                                </a>
                            </div>
                            <Button onClick={onClickBack}>{buttonText}</Button>
                        </div>
                    </div>
                </div>
                <div className='error-footer'>
                    {FooterComponent(footerContent, footerLinks)}
                </div>
            </div>
        </div>
    );
})

export default ErrorPage