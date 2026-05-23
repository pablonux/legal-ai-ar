import React from 'react';
import Language from './Language';
import LoginPattern, { LanguageText } from './Login';
import { ItemDataType } from '@appkit4/react-components';
import './login-generic.scss';
import Form from './Form';

export default function LoginGeneric() {

  const textLanguages: LanguageText = {
    en: {
      name: 'English',
      value: {
        title: 'Welcome to',
        name: 'Product Central',
        loginText: 'Login',
        ssoLoginText: 'SSO Login',
        footerContent : "© 2025 PwC US. All rights reserved. PwC US refers to the US group of member firms and may sometimes refer to the PwC network. Each member firm is a separate legal entity.",
        footerLinks: ['Privacy policy', 'Cookie notice', 'Terms and conditions', 'Customize cookie settings']
      }
    },
    dnk: {
      name: 'Dansk',
      value: {
          title: 'Velkommen til',
          name: 'Produkt Central',
          loginText: 'Log på',
          ssoLoginText: 'SSO Log på',
          footerContent: "© 2025 PwC. Alle rettigheder forbeholdes. PwC henviser til det amerikanske medlemsfirma i PwC-netværket eller et af dets datterselskaber eller tilknyttede selskaber.",
          footerLinks: ['Fortrolighedspolitik', 'Cookie-meddelelse', 'Vilkår og betingelser', 'Tilpas cookie-indstillinger']
      }
    }
  }

  const [language, setLanguage] = React.useState('en');
  let defaultLoginValue: any = {};
  Object.assign(defaultLoginValue, textLanguages[language]?.value);
  const [loginValue, setLoginValue] = React.useState(defaultLoginValue);
  delete loginValue['footerContent'];
  delete loginValue['footerLinks'];
  delete loginValue['loginText'];
  delete loginValue['ssoLoginText'];

  const languages = [
    { label: 'English', value: 'en' },
    { label: 'Dansk', value: 'dnk' }
    // {label: '中文', value: 'cn'},
    // {label: 'Dansk', value: 'dnk'},
    // {label: 'Deutsch', value: 'de-DE'},
    // {label: 'Svensk', value: 'sv'},
    // {label: 'Polskie', value: 'polskie'},
    // {label: 'Italiano', value: 'it'},
    // {label: 'Español', value: 'spanish'},
    // {label: 'Françaiss', value: 'french'},
  ];

  const onSelect = (value: string, item: ItemDataType) => {
    const newlanguage = textLanguages[value]?.value;
    setLoginValue({ ...loginValue, ...newlanguage });
    delete loginValue['footerContent'];
    delete loginValue['footerLinks'];
    setLanguage(value);
  }

  const onSSOLogin = () => {
    // fetch("<api target url>") //replace with your sso login url /appkit4-service/api/iam/fedLogin 
    // .then((response) => response.text())
    // .then((data) => {
    //         data = unescape(data);
    //         if(!data.match(/(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g))
    //             //The url for SSOLogin is not correct
    //             console.log("The url for SSOLogin is not correct");
    //         else
    //             window.location.href = encodeURI(data);
    // })
    // .catch(err => {
    //     //SSOLogin is failed
    //     console.log("SSOLogin is failed", err);
    // })
  };

  const labels = React.useMemo(() => {
    return (textLanguages as LanguageText)[language]?.value;
  }, [language, textLanguages])

  return (
    <LoginPattern className="generic" language={language} textLanguages={textLanguages}
      renderLogin={<Form
        {...loginValue}
        onCancel={() => console.log('login')}
        onOk={onSSOLogin}
        headerLayout="vertical"
        footerLayout="vertical"
        cancelText={labels.loginText}
        okText={labels.ssoLoginText}
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
        extra={
          <Language
            onSelect={onSelect}
            value="en"
            data={languages}
          ></Language>
        }
      ></Form>}
    >
    </LoginPattern>
  )
}
