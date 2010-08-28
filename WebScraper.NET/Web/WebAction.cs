﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebScraper.Web
{
    public interface WebAction
    {
        bool isWaitForEvent();
        void doAction(Agent agent);
        bool validate(Agent agent);
        bool canDoAction(Agent agent);
    }

    public abstract class AbstractWebAction : WebAction
    {
        public bool IsWaitForEvent { get; set; }
        public WebValidator Validator { get; set; }
        public WebValidator CanDoValidator { get; set; }
        public AbstractWebAction()
        {

        }

        public AbstractWebAction(bool waitForEvent = false, WebValidator validator = null, WebValidator canDoValidator = null)
        {
            this.IsWaitForEvent = waitForEvent;
            Validator = validator;
            CanDoValidator = canDoValidator;
        }

        public bool isWaitForEvent()
        {
            return IsWaitForEvent;
        }

        public abstract void doAction(Agent agent);
        public virtual bool validate(Agent agent)
        {
            bool ret = true;
            if (null != Validator)
            {
                ret = Validator.validate(agent);
            }
            return ret;
        }
        public virtual bool canDoAction(Agent agent)
        {
            bool ret = true;
            if (null != CanDoValidator)
            {
                ret = CanDoValidator.validate(agent);
            }
            return ret;
        }
    }

    public class ExtractWebAction<V> : AbstractWebAction
    {
        public DataExtractor<HtmlElement, V> Extractor { get; set; }
        public HtmlElementLocator Locator { get; set; }
        public string ContextKey { get; set; }
        public ExtractWebAction()
        {

        }
        public ExtractWebAction(DataExtractor<HtmlElement, V> extractor = null, string contextKey = null, HtmlElementLocator locator = null)
        {
            Extractor = extractor;
            ContextKey = contextKey;
            Locator = locator;
        }
        public override void doAction(Agent agent)
        {
            HtmlElement element = Locator.locate(agent);
            if (null != element)
            {
                V data = Extractor.extract(element);
                if (null != ContextKey && null != data)
                {
                    agent.RequestContext.Add(ContextKey, data);
                }
            }
        }
    }

    public class SimpleWebAction : AbstractWebAction
    {
        public WebStep Step { get; set; }
        public SimpleWebAction()
        {

        }
        public SimpleWebAction(WebStep step = null, WebValidator validator = null, WebValidator canDoValidator = null, bool waitForEvent = false)
            : base(waitForEvent, validator, canDoValidator)
        {
            Step = step;
        }
        public override void doAction(Agent agent)
        {
            if (null != Step)
            {
                Step.execute(agent);
            }
        }
    }

}
