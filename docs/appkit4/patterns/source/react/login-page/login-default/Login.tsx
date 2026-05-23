import { Footer } from '@appkit4/react-components';
import React, { useState } from 'react';
import classNames from 'classnames';
import './login.scss';

export interface LanguageText {
        [key: string]: {
            name: string,
            value: {
                title: string,
                name: string,
                loginText: string,
                ssoLoginText: string,
                footerContent: string,
                footerLinks: string[]
            }
        }
}
export interface LoginProps {
    className?: string,
    type?: 'default' | 'fractional' | 'generic',
    footerContent?: string,
    footerType?: string,
    footerLinks?: ({
        name: string;
        href: string;
        target: string;
    } | {
        name: string;
        href: string;
        target?: undefined;
    })[],
    renderFooterItem?: (item: { name: string, href: string }, index: number) => React.ReactNode,
    textLanguages?: LanguageText,
    backgroundImage?: string,
    renderLogin?: React.ReactNode,
    children?: React.ReactNode[] | React.ReactNode,
    currentIndex?: number,
    backgroundStyle?: React.CSSProperties,
    darkFontColorFractional?: boolean,
    language?: string
}

export const FooterComponent = (textLanguages: LanguageText, languageInfo: string, footerType: string, footerLinks: {name: string, href: string, target?: string}[], renderFooterItem?: ((item: any, index: number) => React.ReactNode) | undefined) => {
    return (
        <Footer content={(textLanguages as LanguageText)[languageInfo].value.footerContent} type={footerType} links={footerLinks.map((item, index: number) => { return { ...item, name: (textLanguages as LanguageText)[languageInfo].value.footerLinks[index] } })} renderFooterItem={renderFooterItem}></Footer>
    );
}

const LoginPattern = React.forwardRef<HTMLElement, LoginProps>(
    (props: LoginProps, ref) => {
        const {
            className,
            type = 'default',
            footerContent = "© 2025 PwC US. All rights reserved. PwC US refers to the US group of member firms and may sometimes refer to the PwC network. Each member firm is a separate legal entity.",
            footerType = 'links',
            footerLinks = [
                { name: 'Privacy policy', href: '#', target: '_blank' },
                { name: 'Cookie notice', href: '#' },
                { name: 'Terms and conditions', href: '#', target: '_self' },
                { name: 'Customize cookie settings', href: '#', target: '_self' }
            ],
            renderFooterItem = (item: { name: string, href: string, target: string }, index: number) => (
                (<div className="ap-footer-link" key={index}>
                    <a className="ap-link" href={item.href} target={item.target ? item.target : '_blank'}>{item.name}</a>
                    <span className="ap-footer-divider">|</span>
                </div>)
            ),
            textLanguages = {
                en: {
                    name: 'English',
                    value: {
                        title: 'Welcome to',
                        detail: 'It’s nice to have you back.',
                        txtLogin: 'Login',
                        txtSSO: 'SSO Login',
                        footerContent,
                        footerLinks: footerLinks.map(item => item.name)
                    }
                }
            },
            backgroundImage,
            currentIndex = 0,
            darkFontColorFractional = false,
            language = 'en'
        } = props;
        let {
            children,
            renderLogin,
            backgroundStyle
        } = props;
        const [currentIndexInfo, setCurrentIndexInfo] = useState(currentIndex);
        const [languageInfo, setLanguageInfo] = useState('en');
        let languages: { label: string, value: string }[] = [];
        for (var key in textLanguages) {
            if (textLanguages.hasOwnProperty(key)) {
                languages.push({ label: (textLanguages as LanguageText)[key].name, value: key });
            }
        }
        React.useEffect(() => {
            setCurrentIndexInfo(currentIndex);
        }, [currentIndex])
        React.useEffect(() => {
            setLanguageInfo(language);
        }, [language])

    

        backgroundStyle = backgroundImage? { ...backgroundStyle, backgroundImage: `url(${backgroundImage})` } : backgroundStyle;
        return (
            <div className={classNames('ap-pattern-login', className, {
                'fractional': type === 'fractional',
                'dark-font-color': darkFontColorFractional
            })}>
                <div className='login-left-side'>
                    <div className='login-header'>
                        <div className='login-logo'></div>
                    </div>
                    {currentIndexInfo > 0 ? (Array.isArray(children) ? children[currentIndexInfo - 1] : children) : renderLogin}
                    <div className='login-footer'>
                        {FooterComponent(textLanguages as LanguageText, languageInfo, footerType, footerLinks, renderFooterItem)}
                    </div>
                </div>
                <div className='login-right-side'>
                    {type !== 'fractional' && <div className='ap-pattern-login-wrapper' style={backgroundStyle}></div>}
                </div>
                {type === 'fractional' && <div className='ap-pattern-login-wrapper' style={backgroundStyle}></div>}
            </div>
        );
})

export default LoginPattern

